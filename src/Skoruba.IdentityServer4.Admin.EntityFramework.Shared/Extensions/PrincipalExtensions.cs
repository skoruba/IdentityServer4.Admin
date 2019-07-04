using IdentityModel;
using System;
using System.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PrincipalExtensions
    {
        public static TimeZoneInfo GetTimeZone(this ClaimsPrincipal p)
        {
            var zoneinfo = p.FindFirst(JwtClaimTypes.ZoneInfo)?.Value;
            if (zoneinfo == null) zoneinfo = TimeZoneInfo.Local.Id;

            return TimeZoneInfo.FindSystemTimeZoneById(zoneinfo);
        }

        public static string GetTenantApplicationId(this ClaimsPrincipal p)
        {
            var id = p.FindFirst("applicationid")?.Value;
            return id ?? "";
        }

        public static string GetTenantDatabaseName(this ClaimsPrincipal p)
        {
            var name = p.FindFirst("dbname")?.Value;
            return name ?? "";
        }

        public static string GetTenantName(this ClaimsPrincipal p)
        {
            var name = p.FindFirst("tenantname")?.Value;
            return name ?? "";
        }

        public static string GetTenantId(this ClaimsPrincipal p)
        {
            var name = p.FindFirst("tenantid")?.Value;
            return name ?? "";
        }
    }
}