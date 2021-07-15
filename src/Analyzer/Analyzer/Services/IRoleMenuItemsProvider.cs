using Analyzer.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Services
{
    public interface IRoleMenuItemsProvider
    {
        IEnumerable<IMenuItemDefinition> GetMenuItemForRoles(string[] roles);

        /// <summary>
        /// Get MenuItems from internal registered roles
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMenuItemDefinition> GetMenuItemForRoles();
    }
}
