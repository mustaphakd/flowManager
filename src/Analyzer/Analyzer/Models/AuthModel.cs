using Analyzer.Core;
using System;

namespace Analyzer
{
    public class AuthModel
    {
        public AuthModel(string token, string scheme = "bearer")
        {
            Check.NotNull(token, nameof(token));
            Token = token;
            Scheme = scheme;
            LastTimeUsage = DateTime.Now;
        }

        public string Token { get; }
        public string Scheme { get; }

        public DateTime LastTimeUsage { get; set; }
    }
}