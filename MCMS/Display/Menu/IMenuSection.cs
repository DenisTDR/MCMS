using System.Collections.Generic;

namespace MCMS.Display.Menu
{
    public interface IMenuSection
    {
        public List<IMenuItemBase> Items { get; }
    }
}