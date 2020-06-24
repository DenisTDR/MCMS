namespace MCMS.Display.Menu
{
    public abstract class MenuItem
    {
        public int Index { get; set; }
    }
    public interface IMenuItem {
        public int Index { get; set; }
        public string IconClasses { get; set; }
    }

    public static class MenuItemExtensions
    {
        public static T WithIconClasses<T>(this T item, string iconClasses) where T : IMenuItem
        {
            item.IconClasses = iconClasses;
            return item;
        }
    }
}