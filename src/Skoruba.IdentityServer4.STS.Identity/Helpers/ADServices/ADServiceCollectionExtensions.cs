using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using Skoruba.IdentityServer4.STS.Identity.Helpers.ADServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ADServiceCollectionExtensions
    {
        public static IServiceCollection AddActiveDirectoryAuth<TUser, TKey>(this IServiceCollection services, IConfiguration configuration)
            where TUser : IdentityUser<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            services.AddOptions<WindowsAuthConfiguration>()
                .Bind(configuration.GetSection(nameof(WindowsAuthConfiguration))).ValidateDataAnnotations();
            services
                .AddSingleton<ADLogonService>()
                .AddScoped<ADUserInfoExtractor>()
                .AddScoped<ADUserSynchronizer<TUser,TKey>>();

            services.Configure<IISServerOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            return services;
        }
    }
}
