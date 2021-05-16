using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using Worosoft.Xamarin.HttpClientExtensions.Extensions;
using Worosoft.Xamarin.HttpClientExtensions.Models;

namespace Worosoft.Xamarin.HttpClientExtensions.Handlers
{
    /***
     *  //todo: convert to Expression Lambda
     *
     *
     */
    class HoistHandler : MessageProcessingHandler
    {
        public HoistHandler() : base(){}

        public HoistHandler(HttpMessageHandler handler) : base(handler) { }
        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Hoist == null)  return request;

            return (HttpRequestMessage)RequestMethodInfo.Invoke(Hoist, new object[]{ request, cancellationToken });
        }

        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (Hoist == null) return response;

            return (HttpResponseMessage)RequestMethodInfo.Invoke(Hoist, new object[] { response, cancellationToken });
        }

        private Type Info { get; } = typeof(MessageProcessingHandler);

        private MethodInfo RequestMethodInfo { get; } =
            typeof(MessageProcessingHandler).GetMethod("ProcessRequest", System.Reflection.BindingFlags.NonPublic, null, new Type[] { typeof(HttpRequestMessage), typeof(CancellationToken) }, null);

        private MethodInfo ResponseMethodInfo { get; } =
            typeof(MessageProcessingHandler).GetMethod("ProcessResponse", System.Reflection.BindingFlags.NonPublic, null, new Type[] { typeof(HttpResponseMessage), typeof(CancellationToken) }, null);

        public MessageProcessingHandler Hoist { get; set; }

        private HttpClient _client;
        public HttpClient Client
        {
            get { return _client; }
            set {
                if (value == _client) return;

                _client = value;

                if (value == null)
                {
                    this.RemoveClient();
                    return;
                }

                var hash = _client.GetHashCode();
                this.SetClient(hash);
            }
        }
    }
}
