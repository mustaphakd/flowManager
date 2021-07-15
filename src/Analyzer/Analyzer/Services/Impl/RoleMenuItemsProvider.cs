using Analyzer.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;

namespace Analyzer.Services.Impl
{
    class RoleMenuItemsProvider : IRoleMenuItemsProvider
    {
        public IEnumerable<IMenuItemDefinition> GetMenuItemForRoles(string[] roles)
        {
            var menuItemDefinitions = ComputeMenuItemDefinitions(roles);
            return menuItemDefinitions;
        }

        public IEnumerable<IMenuItemDefinition> GetMenuItemForRoles()
        {
            var rolesProvider = DependencyService.Get<IRolesProvider>();
            var roles = rolesProvider.GetRoles();
            return GetMenuItemForRoles(roles);
        }

        private IEnumerable<IMenuItemDefinition> ComputeMenuItemDefinitions(string[] roles)
        {
            var computedMenuItems = new List<IMenuItemDefinition>();

            var roleMenuItemsProvider = DependencyService.Get<IMenuItemsProvider>();
            var roleMenuItems = roleMenuItemsProvider.GetRoleMenuItems();
            var keys = roleMenuItems.Keys;

            foreach(var item in roles)
            {
                if (!keys.Contains(item)) continue;

                var menuItems = roleMenuItems[item];

                foreach (var menuItem in menuItems)
                {
                    if (computedMenuItems.Any(existing =>
                    {
                        return menuItem.Order == existing.Order &&
                           menuItem.NameTranslationKey == existing.NameTranslationKey &&
                           menuItem.DescriptionTranslationKey == existing.DescriptionTranslationKey &&
                           menuItem.Image == existing.Image &&
                           menuItem.ViewType.FullName == existing.ViewType.FullName;

                    })) continue;

                    computedMenuItems.Add(menuItem);
                }
            }

            return computedMenuItems.OrderBy(item => item.Order).ToList();
        }
    }
}
