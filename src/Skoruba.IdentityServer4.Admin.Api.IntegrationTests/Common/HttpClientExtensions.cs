using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using IdentityModel;
using Skoruba.IdentityServer4.Admin.Api.Configuration;
using Skoruba.IdentityServer4.Admin.Api.Middlewares;

namespace Skoruba.IdentityServer4.Admin.Api.IntegrationTests.Common
{
    public static class HttpClientExtensions
    {
        public static void SetAdminClaimsViaHeaders(this HttpClient client, AdminApiConfiguration adminConfiguration)
        {
            var claims = new[]
            {
                new Claim(JwtClaimTypes.Subject, Guid.NewGuid().ToString()),
                new Claim(JwtClaimTypes.Name, Guid.NewGuid().ToString()),
                new Claim(JwtClaimTypes.Role, adminConfiguration.AdministrationRole)
            };

            var token = new JwtSecurityToken(claims: claims);
            var t = new JwtSecurityTokenHandler().WriteToken(token);
            client.DefaultRequestHeaders.Add(AuthenticatedTestRequestMiddleware.TestAuthorizationHeader, t);
        }
    }
}