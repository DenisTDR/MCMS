using System.Collections.Generic;
using MCMS.Helpers;

namespace MCMS.Display.Menu
{
    public class MenuSection : IMenuItem
    {
        public string Name { get; set; }
        public bool IsCollapsed { get; set; }
        public List<IMenuItem> Items = new List<IMenuItem>();

        public string Id { get; } = "menu-section-" + Utils.GenerateRandomHexString();
        public int Index { get; set; }
        public string IconClasses { get; set; }
    }
}