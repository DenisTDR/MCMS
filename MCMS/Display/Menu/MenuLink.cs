using System;
using System.Linq;
using System.Reflection;
using MCMS.Display.Link;

namespace MCMS.Display.Menu
{
    public class MenuLink : MLink, IMenuItem
    {
        public MenuLink(string text, Type controller, MethodInfo action = null) : base(text, controller, action)
        {
        }

        public MenuLink(string text, Type controller, string actionName)
            : this(text, controller, controller.GetMethods().FirstOrDefault(mi => mi.Name == actionName))
        {
        }

        public MenuLink(string text, string url) : base(text, url)
        {
        }

        public int Index { get; set; }
    }
}