using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Utilities
{
    public static class GameMenuHelper
    {
        private static FieldInfo MenuPagesField;
        private static PropertyInfo MenuExtenderPageOverrideProperty;

        static GameMenuHelper()
        {
            var gameMenuType = typeof(GameMenu);
            MenuPagesField = gameMenuType.GetField("pages", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static List<IClickableMenu> GetPages(this GameMenu menu)
        {
            return (List<IClickableMenu>)MenuPagesField.GetValue(menu);
        }

        public static IClickableMenu GetCurrentPage(this GameMenu menu)
        {
            var pages = GetPages(menu);
            var currentPage = pages[menu.currentTab];
            if (currentPage.GetType().FullName.Contains("GameMenuPageExtender"))
            {
                if (MenuExtenderPageOverrideProperty == null)
                {
                    MenuExtenderPageOverrideProperty = currentPage.GetType().GetProperty("CurrentOverride", BindingFlags.Public | BindingFlags.Instance);
                }
                currentPage = (MenuExtenderPageOverrideProperty.GetValue(currentPage) as IClickableMenu) ?? currentPage;
            }
            return currentPage;
        }
    }
}
