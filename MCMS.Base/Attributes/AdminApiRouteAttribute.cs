using System;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Mvc.Routing;

namespace MCMS.Base.Attributes
{
    /// <summary>
    /// Specifies an attribute route on admin api controller.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AdminApiRouteAttribute : Attribute, IRouteTemplateProvider
    {
        public AdminApiRouteAttribute(string template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (template.StartsWith("~") && !template.StartsWith("~/"))
            {
                throw new Exception("A route template can't start with ~ (tilda) not followed by a / (slash).");
            }

            if (template.StartsWith("~/"))
            {
                Template = RoutePrefixes.AdminApiRouteBasePath + template.Substring(2);
            }
            else if (template.StartsWith("/"))
            {
                Template = template;
            }
            else
            {
                Template = RoutePrefixes.AdminApiRouteBasePath + template;
            }
        }

        public string Name { get; }
        public int? Order { get; }
        public string Template { get; set; }
    }
}