using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Extensions
{
    public static class AdminServicesExtensions
    {
        public static IServiceCollection AddAdminServices<TAdminDbContext>(
            this IServiceCollection services)
            where TAdminDbContext : DbContext, IAdminPersistedGrantDbContext, IAdminConfigurationDbContext, IAdminLogDbContext
        {

            return services.AddAdminServices<TAdminDbContext, TAdminDbContext, TAdminDbContext>();
        }

        public static IServiceCollection AddAdminServices<TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext>(this IServiceCollection services)
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
        {
            //Repositories
            services.AddTransient<IClientRepository<TConfigurationDbContext>, ClientRepository<TConfigurationDbContext>>();
            services.AddTransient<IIdentityResourceRepository<TConfigurationDbContext>, IdentityResourceRepository<TConfigurationDbContext>>();
            services.AddTransient<IApiResourceRepository<TConfigurationDbContext>, ApiResourceRepository<TConfigurationDbContext>>();
            services.AddTransient<IPersistedGrantRepository<TPersistedGrantDbContext>, PersistedGrantRepository<TPersistedGrantDbContext>>();
            services.AddTransient<ILogRepository<TLogDbContext>, LogRepository<TLogDbContext>>();

            //Services
            services.AddTransient<IClientService<TConfigurationDbContext>, ClientService<TConfigurationDbContext>>();
            services.AddTransient<IApiResourceService<TConfigurationDbContext>, ApiResourceService<TConfigurationDbContext>>();
            services.AddTransient<IIdentityResourceService<TConfigurationDbContext>, IdentityResourceService<TConfigurationDbContext>>();
            services.AddTransient<IPersistedGrantService<TPersistedGrantDbContext>, PersistedGrantService<TPersistedGrantDbContext>>();
            services.AddTransient<ILogService<TLogDbContext>, LogService<TLogDbContext>>();

            //Resources
            services.AddScoped<IApiResourceServiceResources, ApiResourceServiceResources>();
            services.AddScoped<IClientServiceResources, ClientServiceResources>();
            services.AddScoped<IIdentityResourceServiceResources, IdentityResourceServiceResources>();
            services.AddScoped<IPersistedGrantServiceResources, PersistedGrantServiceResources>();

            return services;
        }
    }
}
