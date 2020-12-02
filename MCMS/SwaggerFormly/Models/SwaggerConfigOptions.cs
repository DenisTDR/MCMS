using System;
using System.IO;
using MCMS.Base.Helpers;

namespace MCMS.SwaggerFormly.Models
{
    public class SwaggerConfigOptions
    {
        internal string Name { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string RouteTemplate { get; set; } = "api/docs/{documentName}/swagger.json";
        public Func<Stream> IndexStreamAction { get; set; }

        public SwaggerUiType UiType { get; set; } = SwaggerUiType.SwaggerUi;

        public string GetEndpointUrl(string prefix = null)
        {
            prefix ??= "/";
            return Utils.UrlCombine(prefix, $"api/docs/{Name}/swagger.json");
        }

        public string GetEndpointName()
        {
            return Title + " " + Version;
        }
    }

    public enum SwaggerUiType
    {
        None,
        SwaggerUi,
        ReDoc,
        Both,
    }

    public static class SwaggerUiTypeExtensions
    {
        public static bool UseSwaggerUi(this SwaggerUiType type)
        {
            return type == SwaggerUiType.SwaggerUi || type == SwaggerUiType.Both;
        }
        public static bool UseReDoc(this SwaggerUiType type)
        {
            return type == SwaggerUiType.ReDoc || type == SwaggerUiType.Both;
        }
    }
}