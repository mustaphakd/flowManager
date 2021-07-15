using Analyzer.Core;
using Analyzer.Models;
using FontAwesome;
using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Services.Impl
{
    sealed class MenuItemsProvider : IMenuItemsProvider
    {
        public MenuItemsProvider()
        {
            RoleMenuItems = new Dictionary<string, IEnumerable<IMenuItemDefinition>>
            {
                { "role1",  new []{ new MenuItemDefinition(1, "DashboardMenuItemTitleKey", FontAwesomeIcons.ThumbsUp, "DashboardMenuItemDescriptionKey", typeof(Pages.DashboardPage)) } },
                { "presi",  new []{ new MenuItemDefinition(2, "UsersMenuItemTitleKey", FontAwesomeIcons.UserCircle, "UsersMenuItemDescriptionKey", typeof(Pages.Users)) } },
                { "admin",  new []{
                    new MenuItemDefinition(3, "ContractsMenuItemTitleKey", FontAwesomeIcons.File, "ContractsMenuItemDescriptionKey", typeof(Pages.DashboardPage)),
                    new MenuItemDefinition(1, "DashboardMenuItemTitleKey", FontAwesomeIcons.ThumbsUp, "DashboardMenuItemDescriptionKey", typeof(Pages.DashboardPage)),
                    new MenuItemDefinition(2, "UsersMenuItemTitleKey", FontAwesomeIcons.UserCircle, "UsersMenuItemDescriptionKey", typeof(Pages.Users))
                } }
            };
        }

        public Dictionary<string, IEnumerable<IMenuItemDefinition>> RoleMenuItems { get; }

        /// <summary>
        /// todo: AppSpecific
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, IEnumerable<IMenuItemDefinition>> GetRoleMenuItems()
        {
            return RoleMenuItems;
        }
    }
}
