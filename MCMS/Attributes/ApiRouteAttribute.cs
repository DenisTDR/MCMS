using System;
using Microsoft.AspNetCore.Mvc.Routing;

namespace MCMS.Attributes
{
    /// <summary>
    /// Specifies an attribute route on a controller.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ApiRouteAttribute : Attribute, IRouteTemplateProvider
    {
        public ApiRouteAttribute(string template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (template.StartsWith("/"))
            {
                Template = "/api" + template;
            }
            else
            {
                Template = "api/" + template;
            }
        }

        public string Name { get; }
        public int? Order { get; }
        public string Template { get; set; }
    }
}