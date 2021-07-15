using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Services
{
    public interface IAuthenticatedUser
    {
        string AuthToken { get; }
        DateTime LastLogin { get; }
        string PasswordHash { get; }

        /// <summary>
        /// Could be either email or userName or any other info used to identify users
        /// </summary>
        string Identifier { get; }

        string[] Roles { get; }

        void RefreshLoginStamp();

    }

    internal class AuthenticatedUser : IAuthenticatedUser
    {
        public AuthenticatedUser(string authToken, string passworHash, string identifier, string[] roles = null )
        {
            Core.Check.NotNull(authToken, nameof(authToken));
            Core.Check.NotNull(passworHash, nameof(passworHash));
            Core.Check.NotNull(identifier, nameof(identifier));

            AuthToken = authToken;
            PasswordHash = PasswordHash;
            Identifier = identifier;
            RefreshLoginStamp();
            Roles = roles ?? Array.Empty<string>();
        }

        public string AuthToken { get; }

        public DateTime LastLogin { get; private set; }

        public string PasswordHash { get; }

        public string Identifier { get; }

        public string[] Roles { get; }

        public void RefreshLoginStamp()
        {
            LastLogin = DateTime.Now;
        }
    }
}
