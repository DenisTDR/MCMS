using System.Collections.Generic;
using MCMS.Helpers;

namespace MCMS.Builder.Menu
{
    public class MenuSection : MenuItem
    {
        public string Name { get; set; }
        public bool IsCollapsed { get; set; }
        public List<MenuItem> Items = new List<MenuItem>();

        public string Id { get; } = "menu-section-" + Utils.GenerateRandomHexString();
    }
}