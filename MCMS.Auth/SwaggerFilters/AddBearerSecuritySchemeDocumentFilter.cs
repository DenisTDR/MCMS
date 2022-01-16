using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MCMS.Auth.SwaggerFilters
{
    public class AddBearerSecuritySchemeDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Check if there is any operation with a Bearer Security Requirement (put by AddAuthorizationHeaderOperationFilter)
            var hasBearerSecurityRequirement = swaggerDoc.Paths.Values.Any(path =>
                path.Operations.Values.Any(op =>
                    op.Security.Any(secReq => secReq.Keys.Any(
                        secScheme => secScheme.Reference.Id == JwtBearerDefaults.AuthenticationScheme))));
            if (hasBearerSecurityRequirement)
            {
                swaggerDoc.Components.SecuritySchemes.Add(
                    JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description =
                            "JWT Authorization header using the Bearer scheme. \n\n " +
                            "Enter the token in the text input below (without the 'Bearer ' prefix)'.",
                    }
                );
            }
        }
    }
}