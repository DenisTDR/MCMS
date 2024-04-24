using MCMS.Display.Link;

namespace MCMS.Display.Menu
{
    public interface IMenuItem : IMenuItemBase, IItemWithRequiredRoles
    {
        public int Index { get; set; }
    }
}