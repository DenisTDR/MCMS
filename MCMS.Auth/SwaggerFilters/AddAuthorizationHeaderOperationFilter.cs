using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MCMS.Auth.SwaggerFilters
{
    public class AddAuthorizationHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var controllerAuthAttributes = context.MethodInfo
                .DeclaringType?
                .GetCustomAttributes(true).OfType<AuthorizeAttribute>().Distinct()
                .ToList() ?? new List<AuthorizeAttribute>();


            var hasJwtAuthAuth =
                controllerAuthAttributes.Any(attr =>
                    attr.AuthenticationSchemes == JwtBearerDefaults.AuthenticationScheme);

            if (!hasJwtAuthAuth) return;

            var hasAllowAnonymousAttrs =
                context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any()
                || context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>()
                    .Any() == true;

            if (hasAllowAnonymousAttrs) return;

            operation.Responses.Add("401", new OpenApiResponse {Description = "Unauthorized"});
            operation.Responses.Add("403", new OpenApiResponse {Description = "Forbidden"});
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }
    }
}