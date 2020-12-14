using System;
using System.Collections.Generic;
using System.Linq;

namespace MCMS.Display.Menu
{
    public class MenuSection : WithUniqueId, IMenuItem
    {
        public string Name { get; set; }
        public bool IsCollapsed { get; set; }
        public List<IMenuItem> Items { get; init; }

        public IEnumerable<IMenuItem> OrderedItems =>
            Items?.OrderBy(i => i.Index) ?? Array.Empty<IMenuItem>() as IEnumerable<IMenuItem>;

        public string Id => UniqueId;
        public int Index { get; set; }
        public string IconClasses { get; set; }
        public string[] RequiredRoles { get; set; }

        public static List<IMenuItem> ItemsList(params IMenuItem[] args)
        {
            return args?.ToList();
        }

        protected override string GetHashSource()
        {
            return Name + IconClasses + string.Join('-', RequiredRoles ?? Array.Empty<string>());
        }
    }
}