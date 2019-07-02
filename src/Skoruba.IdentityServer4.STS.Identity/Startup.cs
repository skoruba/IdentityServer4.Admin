using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.STS.Identity.Helpers;
using System.Reflection;
using Skoruba.IdentityServer4.STS.Identity.DependencyInjection;

namespace Skoruba.IdentityServer4.STS.Identity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
        public ILogger Logger { get; set; }

        public Startup(IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (environment.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
            Environment = environment;
            Logger = loggerFactory.CreateLogger<Startup>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureRootConfiguration(Configuration);

            ///// Add single tenant configuration
            //services.AddSingleTenantConfiguration(Configuration, Environment, Logger,
            //    StartupHelpers.DefaultIdentityDbContextOptions(Configuration),
            //    StartupHelpers.DefaultIdentityOptions(),
            //    StartupHelpers.DefaultIdentityServerOptions(),
            //    StartupHelpers.DefaultIdentityServerConfigurationOptions(Configuration),
            //    StartupHelpers.DefaultIdentityServerOperationalStoreOptions(Configuration)
            //    );

            /// Add multi tenant configuration
            /// Seeding data requires that you build the identity migration using the <see cref="MultiTenantUserIdentityDbContext"/>
            /// The _layout page requires the SignInManager to have the type specified to use the <see cref="MultiTenantUserIdentity"/>
            services.AddMultiTenantConfiguration(Configuration, Environment, Logger,
                StartupHelpers.DefaultIdentityDbContextOptions(Configuration),
                StartupHelpers.DefaultIdentityOptions(),
                StartupHelpers.DefaultIdentityServerOptions(),
                StartupHelpers.DefaultIdentityServerConfigurationOptions(Configuration),
                StartupHelpers.DefaultIdentityServerOperationalStoreOptions(Configuration)
                );

            // Add email senders which is currently setup for SendGrid and SMTP
            services.AddEmailSenders(Configuration);

            // Add services for authentication, including Identity model, IdentityServer4 and external providers
            services.AddAuthenticationServices(Configuration);

            // Add authorization policies for MVC
            services.AddAuthorizationPolicies();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.AddLogging(loggerFactory, Configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add custom security headers
            app.UseSecurityHeaders();

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcLocalizationServices();
            app.UseMvcWithDefaultRoute();
        }
    }
}