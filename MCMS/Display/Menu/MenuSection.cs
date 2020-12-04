using System.Collections.Generic;
using MCMS.Base.Helpers;
using System.Linq;

namespace MCMS.Display.Menu
{
    public class MenuSection : IMenuItem
    {
        public string Name { get; set; }
        public bool IsCollapsed { get; set; }
        public List<IMenuItem> Items = new();
        public string Id { get; } = "menu-section-" + Utils.GenerateRandomHexString();
        public int Index { get; set; }
        public string IconClasses { get; set; }
        public string[] RequiredRoles { get; set; }

        public static List<IMenuItem> ItemsList(params IMenuItem[] args)
        {
            return args?.ToList();
        }
    }
}