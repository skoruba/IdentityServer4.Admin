using IdentityServer4.Admin.MultiTenancy.Implemantation.AspNetCore;
using IdentityServer4.Admin.MultiTenancy.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.Admin.MultiTenancy.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMultiTenantDependencies(this IServiceCollection services)
        {
            services.AddTransient<MultiTenancyMiddleware>();
            services.AddTransient<ICurrentTenant, CurrentTenant>();
            services.AddSingleton<ICurrentTenantAccessor, AsyncLocalCurrentTenantAccessor>();
            services.AddTransient<ITenantResolver, TenantResolver>();

            return services;
        }
    }
}
