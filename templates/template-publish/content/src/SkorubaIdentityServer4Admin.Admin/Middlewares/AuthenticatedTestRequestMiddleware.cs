using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SkorubaIdentityServer4Admin.Admin.Middlewares
{
    public class AuthenticatedTestRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public const string TestUserPrefixHeader = "TestUser";
        public const string TestUserId = "UserId";
        public const string TestUserName = "UserName";
        public const string TestUserRoles = "UserRoles";

        public AuthenticatedTestRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.Keys.Contains($"{TestUserPrefixHeader}-{TestUserName}"))
            {
                var name =
                    context.Request.Headers[$"{TestUserPrefixHeader}-{TestUserName}"].First();

                var id =
                    context.Request.Headers.Keys.Contains($"{TestUserPrefixHeader}-{TestUserId}")
                        ? context.Request.Headers[$"{TestUserPrefixHeader}-{TestUserId}"].First() : string.Empty;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.NameIdentifier, id),
                };

                AddRoles(context, claims);

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                context.User = claimsPrincipal;
            }

            await _next(context);
        }

        private void AddRoles(HttpContext context, List<Claim> claims)
        {
            var roles = context.Request.Headers.Keys.Contains($"{TestUserPrefixHeader}-{TestUserRoles}")
                ? context.Request.Headers[$"{TestUserPrefixHeader}-{TestUserRoles}"].First()
                : string.Empty;

            var rolesList = new List<string>();

            if (!string.IsNullOrEmpty(roles))
            {
                rolesList.AddRange(roles.Split(','));
            }

            claims.AddRange(rolesList.Select(role => new Claim(ClaimTypes.Role, role)));
        }
    }
}