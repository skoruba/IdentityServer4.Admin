using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IdentityServer4.Admin.MultiTenancy.Implemantation.AspNetCore
{
    public static class ServiceProviderExtensions
    {
        public static HttpContext GetHttpContext(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
        }
    }
}
