using System.Linq;
using System.Reflection;
using MCMS.Base.Extensions;
using MCMS.Base.Files.UploadPurpose;
using MCMS.Files.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MCMS.Files.Filters
{
    public class SwaggerFilePurposesFilter : ISchemaFilter
    {
        private readonly ILogger<SwaggerFilePurposesFilter> _logger;
        private readonly UploadPurposeOptions _options;

        public SwaggerFilePurposesFilter(ILogger<SwaggerFilePurposesFilter> logger,
            IOptions<UploadPurposeOptions> options)
        {
            _logger = logger;
            _options = options.Value;
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

                _options.Register(fileAttribute.Purpose, fileAttribute);
            }
        }
    }
}