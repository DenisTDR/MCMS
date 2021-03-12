using System.Collections.Generic;

namespace MCMS.Display.Menu
{
    public class PartialMenuSection : IMenuItemBase, IMenuSection
    {
        public PartialMenuSection(string isExtensionOf)
        {
            IsExtensionOf = isExtensionOf;
        }

        public string IsExtensionOf { get; }
        public List<IMenuItemBase> Items { get; } = new();
    }
}