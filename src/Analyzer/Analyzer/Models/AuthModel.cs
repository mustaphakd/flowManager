using Analyzer.Core;

namespace Analyzer
{
    public class AuthModel
    {
        public AuthModel(string token, string scheme = "bearer")
        {
            Check.NotNull(token, nameof(token));
            Token = token;
            Scheme = scheme;
        }

        public string Token { get; }
        public string Scheme { get; }
    }
}