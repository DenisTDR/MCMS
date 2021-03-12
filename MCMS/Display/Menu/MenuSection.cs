using System;
using System.Collections.Generic;
using System.Linq;
using MCMS.Display.Link;

namespace MCMS.Display.Menu
{
    public class MenuSection : WithUniqueId, IMenuItem, IItemWithIcon, IMenuSection
    {
        public string Name { get; set; }

        public bool IsCollapsable { get; set; }
        public List<IMenuItemBase> Items { get; set; } = new();

        public List<IMenuItem> GetItems()
        {
            return Items.Cast<IMenuItem>().ToList();
        }

        public string Id
        {
            get => UniqueId;
            init => SetCustomId(value);
        }

        public int Index { get; set; }
        public string IconClasses { get; set; }
        public string[] RequiredRoles { get; set; }

        public static List<IMenuItemBase> ItemsList(params IMenuItemBase[] args)
        {
            return args?.ToList();
        }

        protected override string GetHashSource()
        {
            return Name + IconClasses + string.Join('-', RequiredRoles ?? Array.Empty<string>());
        }
    }
}