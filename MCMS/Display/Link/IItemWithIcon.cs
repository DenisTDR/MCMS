namespace MCMS.Display.Link
{
    public interface IItemWithIcon
    {
        public string IconClasses { get; set; }
    }

    public static class ItemWithIconExtensions
    {
        public static T WithIconClasses<T>(this T item, string iconClasses) where T : IItemWithIcon
        {
            item.IconClasses = iconClasses;
            return item;
        }
    }
}