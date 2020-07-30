using MCMS.Display.Link;

namespace MCMS.Display.Menu
{
    public interface IMenuItem : IItemWithIcon
    {
        public int Index { get; set; }
        public string[] RequiredRoles { get; set; }
    }
}