namespace MCMS.Display.Menu
{
    public interface IMenuItem : IMenuItemBase
    {
        public int Index { get; set; }
        public string[] RequiredRoles { get; set; }
    }
}