using System.Collections.Generic;
using MCMS.Base.Helpers;

namespace MCMS.Base.SwaggerFormly.Models
{
    public class SwaggerConfigsOptions
    {
        public readonly List<SwaggerConfigOptions> CustomConfigs = new();
        public SwaggerConfigOptions ForAdmin { get; set; }
        public SwaggerConfigOptions ForApi { get; set; }
        public List<string> JavascriptFiles { get; set; } = new();
        public List<string> StylesheetFiles { get; set; } = new();

        public IEnumerable<SwaggerConfigOptions> GetAll()
        {
            var configs = new List<SwaggerConfigOptions>(CustomConfigs);
            if (ForApi != null)
                configs.Insert(0, ForApi);
            if (ForAdmin != null)
                configs.Insert(0, ForAdmin);
            return configs;
        }

        public void PatchMainConfigs()
        {
            var homeUrl =
                $"<a href='{Utils.UrlCombine(RoutePrefixes.RoutePrefix, RoutePrefixes.AdminRoutePrefix.Substring(1))}'>Back to admin page</a>" +
                "\n<div id='doc-switch-container'></div>";

            ForAdmin.Name = "admin-api";
            ForAdmin.Description = $"{homeUrl}{ForAdmin.Description}";

            if (ForApi != null)
            {
                ForApi.Name = "api";
                ForApi.Description = $"{homeUrl}{ForApi.Description}";
            }

            JavascriptFiles.Add("_content/MCMS/api/docs/swagger-ui-theme.js");
            StylesheetFiles.Add("_content/MCMS/api/docs/swagger-ui-theme.css");
        }

        public void SetupSwallowClone(SwaggerConfigsOptions clone)
        {
            clone.ForAdmin = ForAdmin;
            clone.ForApi = ForApi;
            clone.CustomConfigs.AddRange(CustomConfigs);
            clone.JavascriptFiles.AddRange(JavascriptFiles);
            clone.StylesheetFiles.AddRange(StylesheetFiles);
        }
    }
}