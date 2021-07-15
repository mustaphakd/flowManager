using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;

namespace Worosoft.Xamarin.HttpClientExtensions.Handlers
{
    class LoggingHandler : MessageProcessingHandler
    {
        public LoggingHandler(ILoggerFactory factory) : base(new HttpClientHandler())
        {
            if (factory == null) throw new ArgumentNullException();
            Logger = factory.CreateLogger(nameof(LoggingHandler));
        }
        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Logger.LogDebug("LoggingHandler processing request");

            if(request.Headers.Contains(Constants.LOGGING_CORRELATION_ID))
            {
                return request;
            }

            request.Headers.TryAddWithoutValidation(Constants.LOGGING_CORRELATION_ID, Guid.NewGuid().ToString("N"));

            return request;
        }

        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            Logger.LogDebug("LoggingHandler processing response");
            return response;
        }

        ILogger Logger { get; set; }
    }
}