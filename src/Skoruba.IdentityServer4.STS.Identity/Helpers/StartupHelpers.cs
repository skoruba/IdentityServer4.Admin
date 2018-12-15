using System;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Constants;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public static class StartupHelpers
    {

        public static void AddMvcLocalization(this IServiceCollection services)
        {
            services.AddLocalization(opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddViewLocalization(
                    LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; })
                .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supportedCultures = new[]
                    {
                        new CultureInfo("ru"),
                        new CultureInfo("en")
                    };

                    opts.DefaultRequestCulture = new RequestCulture("en");
                    opts.SupportedCultures = supportedCultures;
                    opts.SupportedUICultures = supportedCultures;
                });
        }

        public static void AddAuthenticationServices<TContext, TUserIdentity, TUserIdentityRole>(this IServiceCollection services, IHostingEnvironment hostingEnvironment, IConfiguration configuration) where TContext : DbContext
            where TUserIdentity : class where TUserIdentityRole : class
        {
            var connectionString = configuration.GetConnectionString(ConfigurationConsts.AdminConnectionStringKey);
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentity<TUserIdentity, TUserIdentityRole>()
                .AddEntityFrameworkStores<TContext>()
                .AddDefaultTokenProviders();

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddAspNetIdentity<TUserIdentity>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    options.EnableTokenCleanup = true;
#if DEBUG
                    options.TokenCleanupInterval = 15;
#endif                
                });

            if (hostingEnvironment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }
        }

        public static void AddDbContexts<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(ConfigurationConsts.AdminConnectionStringKey);
            services.AddDbContext<TContext>(options => options.UseSqlServer(connectionString));
        }

        public static void UseMvcLocalizationServices(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
        }
    }
}
