using System;
using System.Collections.Generic;
using System.Text;

namespace Worosoft.Xamarin.HttpClientExtensions
{
    //https://tools.ietf.org/html/rfc6750
    public static class Constants
    {
        /// <summary>
        /// Use this header to inform the httpClient request pipeline delegates that authentication is being made.
        /// </summary>
        public const string AUTH_REQUEST_HEADER = "WWW-Authenticating";

        //
        /// <summary>
        /// Request lacks any authentication information
        /// HTTP/1.1 401 Unauthorized
        /// WWW-Authenticate: Bearer realm = "example"
        ///
        /// error="invalid_token",
        // error_description="The access token expired"
        /// </summary>
        public const string AUTH_HEADER_REQUIRED = "WWW-Authenticate";


        public const string AUTH_SCHEME = "bearer";

        public const string LOGGING_CORRELATION_ID = "trace-correlation-id";
    }
}
