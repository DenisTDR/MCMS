using System;
using System.Linq;
using System.Reflection;
using MCMS.Base.Extensions;
using MCMS.Files.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MCMS.Files.Filters
{
    public class SwaggerFilePurposesFilter : ISchemaFilter
    {
        private readonly ILogger<SwaggerFilePurposesFilter> _logger;

        public SwaggerFilePurposesFilter(ILogger<SwaggerFilePurposesFilter> logger)
        {
            _logger = logger;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var fileUploadProps = context.Type.GetProperties()
                .Where(prop => prop.GetCustomAttribute<FormlyFileAttribute>() != null).ToList();
            if (!fileUploadProps.Any())
            {
                return;
            }

            foreach (var prop in fileUploadProps)
            {
                var fileAttribute = prop.GetCustomAttribute<FormlyFileAttribute>();
                if (string.IsNullOrEmpty(fileAttribute?.Purpose))
                {
                    _logger.LogError("Found a null purposed FormlyFileField: " + context.Type.CSharpName() + " -> " +
                                     prop.Name);
                    continue;
                }

                if (!MFilesSpecifications.RegisteredPurposes.ContainsKey(fileAttribute.Purpose))
                {
                    MFilesSpecifications.RegisteredPurposes[fileAttribute.Purpose] = fileAttribute;
                }
            }
        }
    }
}