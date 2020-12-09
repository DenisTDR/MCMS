using System;
using System.IO;
using MCMS.Base.Helpers;
using Microsoft.OpenApi.Models;

namespace MCMS.Base.SwaggerFormly.Models
{
    public class SwaggerConfigOptions
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string RouteTemplate { get; set; } = "api/docs/{documentName}/swagger.json";
        public Func<Stream> IndexStreamAction { get; set; }

        public DocsUiType UiType { get; set; } = DocsUiType.SwaggerUi;

        public string GetEndpointUrl(string prefix = null)
        {
            prefix ??= "/";
            return Utils.UrlCombine(prefix, $"api/docs/{Name}/swagger.json");
        }

        public string GetEndpointName()
        {
            return Title + " " + Version;
        }

        public OpenApiInfo ToOpenApiInfo()
        {
            return new OpenApiInfo
            {
                Title = Title,
                Version = Version,
                Description = Description,
            };
        }
    }

    public enum DocsUiType
    {
        None,
        SwaggerUi,
        ReDoc,
        Both,
    }

    public static class SwaggerUiTypeExtensions
    {
        public static bool UseSwaggerUi(this SwaggerConfigOptions config)
        {
            return config != null && (config.UiType == DocsUiType.SwaggerUi || config.UiType == DocsUiType.Both);
        }

        public static bool UseReDoc(this SwaggerConfigOptions config)
        {
            return config != null && (config.UiType == DocsUiType.ReDoc || config.UiType == DocsUiType.Both);
        }
    }
}