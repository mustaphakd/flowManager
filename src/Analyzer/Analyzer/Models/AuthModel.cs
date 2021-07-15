using Analyzer.Core;
using System;

namespace Analyzer
{
    public class AuthModel
    {
        public AuthModel(string token, string scheme = "bearer", string[] roles = null)
        {
            Check.NotNull(token, nameof(token));
            Token = token;
            Scheme = scheme;
            LastTimeUsage = DateTime.Now;
            Roles = roles;
        }

        public string Token { get; }
        public string Scheme { get; }

        public string[] Roles { get; }

        public DateTime LastTimeUsage { get; set; }
    }
}