using System;
using System.Linq;
using System.Reflection;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Display.Link
{
    public class MLink : IItemWithIcon
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
        public string[] RequiredRoles { get; set; }

        public virtual string BuildUrl(IUrlHelper urlHelper = null)
        {
            if (urlHelper == null || Controller == null)
            {
                return Url;
            }

            return urlHelper.ActionLink(ActionName, ControllerName, protocol: Utils.GetExternalProtocol());
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
}