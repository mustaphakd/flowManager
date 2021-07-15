using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Forms;

namespace Worosoft.Xamarin.HttpClientExtensions
{
    public class AllInclusiveFactory : IHttpClientFactory
    {
        const string defaultClientName = "wrsft_default_client_@";
        static AllInclusiveFactory()
        {

            DependencyService.Register<IHttpClientFactory, AllInclusiveFactory>();
        }

        public HttpClient CreateClient(string name)
        {
            var clientName = name ?? defaultClientName;

            if(ClientsCache.TryGetValue(clientName, out HttpClient client))
            {
                return client;
            }

            var loggerFactory = DependencyService.Resolve<ILoggerFactory>();

            var loggingHandler = new Handlers.LoggingHandler(loggerFactory);
            var hoistHandler = new Handlers.HoistHandler(loggingHandler);

            Func<HttpRequestHeaders> httpRequestHeaderAccessor = () => {
                var key = clientName;

                if(ClientsCache.TryGetValue(key, out HttpClient httpClient))
                {
                    var requestHeaders = httpClient.DefaultRequestHeaders;
                    return requestHeaders;
                }

                throw new InvalidOperationException($"httpclient does not existing for {key}");
            };

            var bearerHandler = new Handlers.BearerAuthorizationHandler( httpRequestHeaderAccessor, hoistHandler);
            var client2 = new HttpClient(bearerHandler);

            /*
             *  allows for setting api specific handler when building service for Api
             * **/
            hoistHandler.Client = client2;

            ClientsCache.Add(clientName, client2);

            return client2;
        }

        public HttpClient TryGetHttpClient(string name)
        {
            if(ClientsCache.TryGetValue(name, out HttpClient client))
            {
                return client;
            }

            return null;
        }


        private static Dictionary<string, HttpClient> ClientsCache { get; } = new Dictionary<string, HttpClient>();
    }
}
