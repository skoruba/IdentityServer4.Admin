using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity;
using SkorubaIdentityServer4Admin.STS.Identity.Helpers;

namespace SkorubaIdentityServer4Admin.STS.Identity
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
            services.AddDbContexts<AdminDbContext>(Configuration);
            services.AddAuthenticationServices<AdminDbContext, UserIdentity, UserIdentityRole>(Environment, Configuration, Logger);
            services.AddMvcLocalization();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
			app.AddLogging(loggerFactory, Configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSecurityHeaders();
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcLocalizationServices();
            app.UseMvcWithDefaultRoute();
        }
    }
}
