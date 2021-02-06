using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MCMS.SwaggerFormly.Filters
{
    public class DefaultSummaryOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!string.IsNullOrEmpty(operation.Summary))
            {
                return;
            }

            operation.Summary = context.ApiDescription.RelativePath;
        }
    }
}