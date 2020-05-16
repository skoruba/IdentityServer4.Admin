using Microsoft.AspNetCore.Http;
using System;

namespace Skoruba.MultiTenant.Finbuckle.Configuration
{
    public static class MultiTenantHelper
    {
        public static void SetTenantCookie(HttpResponse response, string tenantKey, string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                response.Cookies.Delete(tenantKey);
            }
            else
            {
                //TODO: encrypt value
                response.Cookies.Append(
                    tenantKey,
                    tenantId.ToString(),
                    new CookieOptions
                    {
                        Path = "/",
                        HttpOnly = false,
                        Expires = DateTimeOffset.Now.AddYears(10)
                    }
                );
            }
        }
    }
}