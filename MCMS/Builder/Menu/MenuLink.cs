using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Builder.Menu
{
    public class MenuLink
    {
        public string Text { get; }
        public string Url { get; }
        public Type Controller { get; }
        public MethodInfo Action { get; private set; }
        public string Target { get; private set; }
        public string ControllerName => Controller != null ? Controller.Name.Replace("Controller", "") : null;
        public string ActionName => Action != null ? Action.Name : "Index";
        public string FaIconClasses { get; private set; }

        public string GetUrl(IUrlHelper urlHelper = null)
        {
            if (urlHelper == null || Controller == null)
            {
                return Url;
            }

            return urlHelper.ActionLink(ActionName, ControllerName);
        }
        public MenuLink(string text, Type controller, MethodInfo action = null)
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

        public MenuLink(string text, Type controller, string actionName)
            : this(text, controller, controller.GetMethods().FirstOrDefault(mi => mi.Name == actionName))
        {
        }

        public MenuLink(string text, string url)
        {
            Text = text;
            Url = url;
        }

        public MenuLink WithMethod(string actionName)
        {
            return WithMethod(Controller?.GetMethod(actionName));
        }

        public MenuLink WithMethod(MethodInfo action)
        {
            Action = action;
            return this;
        }

        public MenuLink WithTarget(string target)
        {
            Target = target;
            return this;
        }

        public MenuLink WithFaIconClasses(string faIconClasses)
        {
            FaIconClasses = faIconClasses;
            return this;
        }
    }
}