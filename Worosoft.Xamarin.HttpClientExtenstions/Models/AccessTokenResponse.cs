using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Worosoft.Xamarin.HttpClientExtensions.Models
{
    public class AccessTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string Token { get; set; }

        [JsonPropertyName("token_type")]
        public string Type { get; set; }

        [JsonPropertyName("expires_in")]
        public int Expiration { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
