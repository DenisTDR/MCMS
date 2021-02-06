using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MCMS.Auth
{
    public class AddBearerSecuritySchemeDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Check if there is any operation with a Bearer Security Requirement (put by AddAuthorizationHeaderOperationFilter)
            var hasBearerSecurityRequirement = swaggerDoc.Paths.Values.Any(path =>
                path.Operations.Values.Any(op =>
                    op.Security.Any(secReq => secReq.Keys.Any(
                        secScheme => secScheme.Reference.Id == "Bearer"))));
            if (hasBearerSecurityRequirement)
            {
                swaggerDoc.Components.SecuritySchemes.Add(
                    "Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description =
                            "JWT Authorization header using the Bearer scheme. \n\n " +
                            "Enter your token in the text input below (without the 'Bearer ' part)'.",
                    }
                );
            }
        }
    }
}