using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Services
{
    public interface IRolesProvider
    {
        string[] GetRoles();
    }
}
