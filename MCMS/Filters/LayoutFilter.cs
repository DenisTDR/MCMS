using System;
using System.Linq;
using System.Reflection;
using MCMS.Attributes;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MCMS.Filters
{
    public class LayoutFilter : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var attr = actionDescriptor?.MethodInfo.GetCustomAttributes<ViewLayoutAttribute>().ToList();
            var name = attr?.FirstOrDefault()?.LayoutName;
            if (!string.IsNullOrEmpty(name))
            {
                context.HttpContext.Items["ForcedLayout"] = name;
            }
        }
    }
}