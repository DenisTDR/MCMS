using System.Reflection;

namespace MCMS.Display.Link
{
    public static class MLinkExtensions
    {
        public static T WithAction<T>(this T link, string actionName) where T : MLink
        {
            return link.WithAction(link.Controller?.GetMethod(actionName));
        }

        public static T WithAction<T>(this T link, MethodInfo action) where T : MLink
        {
            link.Action = action;
            return link;
        }

        public static T WithTarget<T>(this T link, string target) where T : MLink
        {
            link.Target = target;
            return link;
        }
        
        public static T WithTag<T>(this T item, string tag) where T : MLink
        {
            item.Tag = tag;
            return item;
        }

        public static T WithTitle<T>(this T item, string title) where T : MLink
        {
            item.Title = title;
            return item;
        }

        public static string FontAwesomeIconHtml<T>(this T link) where T : MLink
        {
            return link.IconClasses == null ? null : "<i class=\"" + link.IconClasses + " fa-fw\"></i>";
        }

        public static string BuildText<T>(this T link) where T : MLink
        {
            return link.FontAwesomeIconHtml() + " " + link.Text;
        }
    }
}