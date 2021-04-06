using System.Collections.Generic;
using System.Linq;
using MCMS.Base.Exceptions;

namespace MCMS.Display.Menu
{
    public class MenuConfig
    {
        private readonly List<IMenuItemBase> _items = new();

        private List<IMenuItem> _preparedItems;

        public void Add(IEnumerable<IMenuItemBase> items)
        {
            _items.AddRange(items);
        }

        public void Add(IMenuItemBase item)
        {
            _items.Add(item);
        }

        public void Add(params IMenuItemBase[] items)
        {
            _items.AddRange(items);
        }


        public List<IMenuItem> GetPreparedItems()
        {
            return _preparedItems ??= PrepareItems(_items);
        }

        private List<IMenuItem> PrepareItems(List<IMenuItemBase> items)
        {
            if (items.Any(item => item is PartialMenuSection))
            {
                var partials = items.Where(item => item is PartialMenuSection).Cast<PartialMenuSection>();
                items = items.Where(item => !(item is PartialMenuSection)).ToList();
                foreach (var partialMenuSection in partials)
                {
                    if (!(items.FirstOrDefault(item =>
                        item is MenuSection ms && ms.Id == partialMenuSection.IsExtensionOf) is MenuSection targetItem))
                    {
                        throw new KnownException("MenuSection with id `" + partialMenuSection.IsExtensionOf +
                                                 "` not found. It is needed for a PartialMenuSection targeting to it.");
                    }

                    targetItem.Items.AddRange(partialMenuSection.Items);
                }
            }

            foreach (var item in items.Where(item => item is MenuSection).Cast<MenuSection>())
            {
                item.Items = PrepareItems(item.Items).Cast<IMenuItemBase>().ToList();
            }

            return items.Cast<IMenuItem>().OrderBy(item => item.Index).ToList();
        }
    }
}