using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyzer.Services.Impl
{
    class RolesProvider : IRolesProvider
    {
        public RolesProvider(IEnumerable<string> roles)
        {
            Roles = roles.ToArray();
        }

        public string[] Roles { get; }

        public string[] GetRoles()
        {
            return Roles;
        }
    }
}
