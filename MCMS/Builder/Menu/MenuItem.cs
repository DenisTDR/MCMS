namespace MCMS.Builder.Menu
{
    public abstract class MenuItem
    {
        public string IconClasses { get; set; }
        //
        // public MenuItem WithIconClasses(string faIconClasses)
        // {
        //     IconClasses = faIconClasses;
        //     return this;
        // }
    }

    public static class MenuItemExtensions
    {
        public static T WithIconClasses<T>(this T item, string iconClasses) where T : MenuItem
        {
            item.IconClasses = iconClasses;
            return item;
        }
    }
}