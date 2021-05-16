using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Worosoft.Xamarin.HttpClientExtensions.Models;

namespace Worosoft.Xamarin.HttpClientExtensions.Handlers
{
    class BearerAuthorizationHandler : MessageProcessingHandler
    {
        public BearerAuthorizationHandler(Func<HttpRequestHeaders> defaultRequestHeaderAccessor) : this(defaultRequestHeaderAccessor, null)
        {
        }

        public BearerAuthorizationHandler(Func<HttpRequestHeaders> defaultRequestHeaderAccessor, HttpMessageHandler handler): base(handler)
        {
            if (defaultRequestHeaderAccessor == null) throw new ArgumentNullException();
        }

        /// <summary>
        ///  Right now other Authentication scheme scenario aren't supported.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorization = request.Headers.Authorization;

            if( ! String.IsNullOrEmpty(Token) &&
                (  authorization == null ||
                ! String.Equals(authorization.Scheme, Constants.AUTH_SCHEME, StringComparison.InvariantCultureIgnoreCase)
                ))
            {
                var bearer = new AuthenticationHeaderValue(Constants.AUTH_SCHEME, Token);
                request.Headers.Authorization = bearer;
            }
            else if(request.Headers.Contains(Constants.AUTH_REQUEST_HEADER))
            {
                AuthenticationRequestBeingMade = true;
            }

            return request;
        }

        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (AuthenticationRequestBeingMade == true && response.IsSuccessStatusCode == true)
            {
                AuthenticationRequestBeingMade = false;
                var contentLength = response.Content.Headers.ContentLength;
                var buffer = new byte[contentLength.Value];
                using var strm = new MemoryStream(buffer);
                {
                    response.Content.CopyToAsync(strm).ConfigureAwait(true).GetAwaiter().GetResult();
                }

                var json = System.Text.Encoding.UTF8.GetString(buffer);
                var accessTokenResponse = System.Text.Json.JsonSerializer.Deserialize<AccessTokenResponse>(json);
                Token = accessTokenResponse.Token;

                var requestHeaders = RequestHeadersAccessor();
                var bearer = new AuthenticationHeaderValue(Constants.AUTH_SCHEME, Token);
                requestHeaders.Authorization = bearer;
            }

                return response;
        }

        public bool AuthenticationRequestBeingMade { get; private set; } = false;

        public string Token { get; private set; } = null;

        public Func<HttpRequestHeaders> RequestHeadersAccessor { get; private set; }
    }
}
