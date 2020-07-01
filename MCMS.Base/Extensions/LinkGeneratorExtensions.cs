using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Routing;

namespace MCMS.Base.Extensions
{
    public static class LinkGeneratorExtensions
    {
        public static string GetAbsolutePathByAction(this LinkGenerator linkGenerator, string actionName,
            string controllerName, object values = null)
        {
            var path = linkGenerator.GetPathByAction(actionName, controllerName, values);
            if (path.StartsWith("/")) path = path.Substring(1);
            var result = Utils.UrlCombine(Env.GetOrThrow("EXTERNAL_URL"), path);
            return result;
        }
    }
}