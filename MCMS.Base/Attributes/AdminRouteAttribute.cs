using System;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Mvc.Routing;

namespace MCMS.Base.Attributes
{
    /// <summary>
    /// Specifies an attribute route on a admin ui controller.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AdminRouteAttribute : Attribute, IRouteTemplateProvider
    {
        public AdminRouteAttribute(string template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (template.StartsWith("/"))
            {
                Template = "/" + GetBasePath() + template;
            }
            else
            {
                Template = GetBasePath() + "/" + template;
            }
        }

        public string Name { get; }
        public int? Order { get; }
        public string Template { get; set; }

        public static string GetBasePath()
        {
            return Env.Get("ADMIN_ROUTE_PREFIX");
        }
    }
}