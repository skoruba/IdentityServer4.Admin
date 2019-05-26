using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Skoruba.IdentityServer4.Admin.Api.Configuration.Constants;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Skoruba.IdentityServer4.Admin.Api.Configuration.Authorization
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>().Any();

            if (hasAuthorize)
            {
                operation.Responses.Add("401", new Response { Description = "Unauthorized" });
                operation.Responses.Add("403", new Response { Description = "Forbidden" });

                operation.Security = new List<IDictionary<string, IEnumerable<string>>> {
                    new Dictionary<string, IEnumerable<string>> {{"oauth2", new[] { AuthorizationConsts.ApiName } }}
                };
            }
        }
    }
}