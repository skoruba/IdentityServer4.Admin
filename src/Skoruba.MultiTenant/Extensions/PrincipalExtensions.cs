using System.Security.Principal;

namespace System.Security.Claims
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetTenantId(this IPrincipal p)
        {
            var name = (p as ClaimsPrincipal)?.FindFirst(Skoruba.MultiTenant.Claims.ClaimTypes.TenantId)?.Value;
            return name ?? "";
        }
    }
}