using System.Collections.Generic;
using System.Linq;

namespace MCMS.Display.Link
{
    public static class MRichLinkExtensions
    {
        public static T AsButton<T>(this T value, string cssClasses) where T : MRichLink
        {
            value.CssClasses = "btn btn-" + cssClasses;
            return value;
        }

        public static T ToggleModal<T>(this T value, bool isWith) where T : MRichLink
        {
            if (value.GetData("toggle") is string str && str == "ajax-modal")
            {
                if (!isWith)
                {
                    value.RemoveData("toggle");
                }
            }
            else if (isWith)
            {
                value.SetData("toggle", "ajax-modal");
            }

            return value;
        }

        public static T WithModal<T>(this T value, string backdrop = "static", bool keyboard = false) where T : MRichLink
        {
            value.SetData("toggle", "ajax-modal");
            value.SetData("modal-backdrop", backdrop);
            value.SetData("modal-keyboard", keyboard.ToString().ToLower());
            return value;
        }

        public static T WithValues<T>(this T value, object values) where T : MRichLink
        {
            value.Values = values;
            return value;
        }

        public static T WithData<T>(this T link, string key, object value) where T : MRichLink
        {
            link.SetData(key, value);
            return link;
        }

        public static void SetData<T>(this T link, string key, object value) where T : MRichLink
        {
            link.AnchorData ??= new();
            link.AnchorData[key] = value;
        }

        public static object GetData<T>(this T link, string key) where T : MRichLink
        {
            if (link.AnchorData == null || !link.AnchorData.ContainsKey(key))
            {
                return null;
            }

            return link.AnchorData[key];
        }

        public static Dictionary<string, object> GetData<T>(this T link) where T : MRichLink
        {
            return link.AnchorData ?? new Dictionary<string, object>();
        }

        public static void RemoveData<T>(this T link, string key) where T : MRichLink
        {
            if (link.AnchorData != null && link.AnchorData.ContainsKey(key))
            {
                link.AnchorData.Remove(key);
            }
        }

        public static bool HasData<T>(this T link) where T : MRichLink
        {
            return link.AnchorData != null && link.AnchorData.Any();
        }
    }
}