using System;
using System.Linq;
using System.Reflection;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.Link
{
    public class MLink
    {
        public string Text { get; }
        public string Url { get; }
        public Type Controller { get; }
        public MethodInfo Action { get; internal set; }
        public string Target { get; internal set; }
        public string ControllerName => Controller != null ? TypeHelpers.GetControllerName(Controller) : null;
        public string ActionName => Action != null ? Action.Name : "Index";
        public string IconClasses { get; set; }
        public string Tag { get; set; }

        public virtual string BuildUrl(IUrlHelper urlHelper = null)
        {
            if (urlHelper == null || Controller == null)
            {
                return Url;
            }

            return urlHelper.ActionLink(ActionName, ControllerName);
        }

        public MLink(string text, Type controller, MethodInfo action = null)
        {
            if (controller != null)
            {
                if (action == null)
                {
                    action = controller.GetMethods().FirstOrDefault(mi => mi.Name == "Index");
                }
            }

            Text = text;
            Controller = controller;
            Action = action;
        }

        public MLink(string text, Type controller, string actionName)
            : this(text, controller, controller.GetMethods().FirstOrDefault(mi => mi.Name == actionName))
        {
        }

        public MLink(string text, string url)
        {
            Text = text;
            Url = url;
        }
    }

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

        public static T WithIconClasses<T>(this T item, string iconClasses) where T : MLink
        {
            item.IconClasses = iconClasses;
            return item;
        }

        public static T WitTag<T>(this T item, string tag) where T : MLink
        {
            item.Tag = tag;
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