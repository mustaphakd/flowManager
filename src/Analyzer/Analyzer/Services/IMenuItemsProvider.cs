using Analyzer.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Services
{
    public interface IMenuItemsProvider
    {
        IDictionary<string, IEnumerable<IMenuItemDefinition>> GetRoleMenuItems();
    }
}
