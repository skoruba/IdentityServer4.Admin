using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Skoruba.IdentityServer4.Admin.Api.Configuration.Authorization
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        private readonly AdminApiConfiguration _adminApiConfiguration;

        public AuthorizeCheckOperationFilter(AdminApiConfiguration adminApiConfiguration)
        {
            _adminApiConfiguration = adminApiConfiguration;
        }
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>().Any();

            if (hasAuthorize)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
                var oAuthScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth2"
                    }
                };
                operation.Security = new List<OpenApiSecurityRequirement> {
                    new OpenApiSecurityRequirement {
                        [oAuthScheme] = new[] { _adminApiConfiguration.OidcApiName }
                    }
                };
            }
        }
    }
}