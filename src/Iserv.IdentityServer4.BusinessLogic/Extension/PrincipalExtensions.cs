using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Iserv.IdentityServer4.BusinessLogic.Extension
{
    public static class PrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return Guid.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub));
        }
    }
}