using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.Middlewares;
using Skoruba.IdentityServer4.Admin.Configuration;
using Skoruba.IdentityServer4.Admin.Configuration.ApplicationParts;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Skoruba.IdentityServer4.Admin.Helpers.Localization;

namespace Skoruba.IdentityServer4.Admin.Helpers
{
    public static class StartupHelpers
    {
        public static Action<DbContextOptionsBuilder> DefaultIdentityDbContextOptions(IConfiguration c) =>
            o => o.UseSqlServer(c.GetConnectionString(ConfigurationConsts.IdentityDbConnectionStringKey),
                sql => sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));

        public static Action<ConfigurationStoreOptions> DefaultIdentityServerConfigurationOptions(IConfiguration c) =>
            o => o.ConfigureDbContext = b =>
                   b.UseSqlServer(c.GetConnectionString(ConfigurationConsts.ConfigurationDbConnectionStringKey),
                        sql => sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));

        public static Action<OperationalStoreOptions> DefaultIdentityServerOperationalStoreOptions(IConfiguration c) =>
            o => o.ConfigureDbContext = b =>
                b.UseSqlServer(c.GetConnectionString(ConfigurationConsts.PersistedGrantDbConnectionStringKey),
                    sql => sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));

        public static Action<DbContextOptionsBuilder> DefaultLogDbContextOptions(IConfiguration c) =>
            o => o.UseSqlServer(c.GetConnectionString(ConfigurationConsts.AdminLogDbConnectionStringKey),
                sql => sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));

        public static Action<IdentityOptions> DefaultIdentityOptions(IConfiguration c) =>
            o => o.User.RequireUniqueEmail = true;

        ///// <summary>
        ///// Register shared DbContext for IdentityServer ConfigurationStore and PersistedGrants, Identity and Logging
        ///// Configure the connection string in AppSettings.json - use AdminConnection key
        ///// </summary>
        ///// <typeparam name="TContext"></typeparam>
        ///// <param name="services"></param>
        ///// <param name="configuration"></param>
        //public static void RegisterDbContexts<TContext>(this IServiceCollection services, IConfigurationRoot configuration)
        //    where TContext : DbContext
        //{
        //    var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

        //    var operationalStoreOptions = new OperationalStoreOptions();
        //    services.AddSingleton(operationalStoreOptions);

        //    var storeOptions = new ConfigurationStoreOptions();
        //    services.AddSingleton(storeOptions);

        //    services.AddDbContext<TContext>(options => options.UseSqlServer(configuration.GetConnectionString(ConfigurationConsts.AdminConnectionStringKey), optionsSql => optionsSql.MigrationsAssembly(migrationsAssembly)));
        //}

        ///// <summary>
        ///// Register shared in Memory DbContext for IdentityServer ConfigurationStore and PersistedGrants, Identity and Logging
        ///// For testing purpose only
        ///// </summary>
        ///// <typeparam name="TContext"></typeparam>
        ///// <param name="services"></param>
        //public static void RegisterDbContextsStaging<TContext>(this IServiceCollection services)
        //    where TContext : DbContext
        //{
        //    var databaseName = Guid.NewGuid().ToString();

        //    var operationalStoreOptions = new OperationalStoreOptions();
        //    services.AddSingleton(operationalStoreOptions);

        //    var storeOptions = new ConfigurationStoreOptions();
        //    services.AddSingleton(storeOptions);

        //    services.AddDbContext<TContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(databaseName));
        //}

        ///// <summary>
        ///// Register DbContexts for IdentityServer ConfigurationStore and PersistedGrants, Identity and Logging
        ///// Configure the connection strings in AppSettings.json
        ///// </summary>
        ///// <typeparam name="TConfigurationDbContext"></typeparam>
        ///// <typeparam name="TPersistedGrantDbContext"></typeparam>
        ///// <typeparam name="TLogDbContext"></typeparam>
        ///// <param name="services"></param>
        ///// <param name="configuration"></param>
        //public static void RegisterDbContexts<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext>(this IServiceCollection services, IConfigurationRoot configuration)
        //    where TIdentityDbContext : DbContext
        //    where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
        //    where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
        //    where TLogDbContext : DbContext, IAdminLogDbContext
        //{
        //    var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

        //    // Config DB for identity
        //    services.AddDbContext<TIdentityDbContext>(options =>
        //        options.UseSqlServer(configuration.GetConnectionString(ConfigurationConsts.IdentityDbConnectionStringKey),
        //            sql => sql.MigrationsAssembly(migrationsAssembly)));

        //    // Config DB from existing connection
        //    services.AddConfigurationDbContext<TConfigurationDbContext>(options =>
        //    {
        //        options.ConfigureDbContext = b =>
        //            b.UseSqlServer(configuration.GetConnectionString(ConfigurationConsts.ConfigurationDbConnectionStringKey),
        //                sql => sql.MigrationsAssembly(migrationsAssembly));
        //    });

        //    // Operational DB from existing connection
        //    services.AddOperationalDbContext<TPersistedGrantDbContext>(options =>
        //    {
        //        options.ConfigureDbContext = b =>
        //            b.UseSqlServer(configuration.GetConnectionString(ConfigurationConsts.PersistedGrantDbConnectionStringKey),
        //                sql => sql.MigrationsAssembly(migrationsAssembly));
        //    });

        //    // Log DB from existing connection
        //    services.AddDbContext<TLogDbContext>(options =>
        //        options.UseSqlServer(
        //            configuration.GetConnectionString(ConfigurationConsts.AdminLogDbConnectionStringKey),
        //            optionsSql => optionsSql.MigrationsAssembly(migrationsAssembly)));
        //}

        ///// <summary>
        ///// Register in memory DbContexts for IdentityServer ConfigurationStore and PersistedGrants, Identity and Logging
        ///// For testing purpose only
        ///// </summary>
        ///// <typeparam name="TConfigurationDbContext"></typeparam>
        ///// <typeparam name="TPersistedGrantDbContext"></typeparam>
        ///// <typeparam name="TLogDbContext"></typeparam>
        ///// <typeparam name="TIdentityDbContext"></typeparam>
        ///// <param name="services"></param>
        //public static void RegisterDbContextsStaging<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext>(this IServiceCollection services)
        //    where TIdentityDbContext : DbContext
        //    where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
        //    where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
        //    where TLogDbContext : DbContext, IAdminLogDbContext
        //{
        //    var persistedGrantsDatabaseName = Guid.NewGuid().ToString();
        //    var configurationDatabaseName = Guid.NewGuid().ToString();
        //    var logDatabaseName = Guid.NewGuid().ToString();
        //    var identityDatabaseName = Guid.NewGuid().ToString();

        //    var operationalStoreOptions = new OperationalStoreOptions();
        //    services.AddSingleton(operationalStoreOptions);

        //    var storeOptions = new ConfigurationStoreOptions();
        //    services.AddSingleton(storeOptions);

        //    services.AddDbContext<TIdentityDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(identityDatabaseName));
        //    services.AddDbContext<TPersistedGrantDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(persistedGrantsDatabaseName));
        //    services.AddDbContext<TConfigurationDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(configurationDatabaseName));
        //    services.AddDbContext<TLogDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(logDatabaseName));
        //}

        /// <summary>
        /// Using of Forwarded Headers, Hsts, XXssProtection and Csp
        /// </summary>
        /// <param name="app"></param>
        public static void UseSecurityHeaders(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseHsts(options => options.MaxAge(days: 365));
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXContentTypeOptions();
            app.UseXfo(options => options.SameOrigin());
            app.UseReferrerPolicy(options => options.NoReferrer());
            var allowCspUrls = new List<string>
            {
                "https://fonts.googleapis.com/",
                "https://fonts.gstatic.com/"
            };

            app.UseCsp(options =>
            {
                options.FontSources(configuration =>
                {
                    configuration.SelfSrc = true;
                    configuration.CustomSources = allowCspUrls;
                });

                //TODO: consider remove unsafe sources - currently using for toastr inline scripts in Notification.cshtml
                options.ScriptSources(configuration =>
                {
                    configuration.SelfSrc = true;
                    configuration.UnsafeInlineSrc = true;
                    configuration.UnsafeEvalSrc = true;
                });

                options.StyleSources(configuration =>
                {
                    configuration.SelfSrc = true;
                    configuration.CustomSources = allowCspUrls;
                    configuration.UnsafeInlineSrc = true;
                });
            });
        }

        /// <summary>
        /// Use default authentication middleware and middleware for integration testing
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public static void ConfigureAuthenticationServices(this IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsStaging())
            {
                app.UseMiddleware<AuthenticatedTestRequestMiddleware>();
            }
        }

        /// <summary>
        /// Add middleware for localization
        /// </summary>
        /// <param name="app"></param>
        public static void ConfigureLocalization(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
        }

        /// <summary>
        /// Add logging configuration
        /// </summary>
        /// <param name="app"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        public static void AddLogging(this IApplicationBuilder app, ILoggerFactory loggerFactory, IConfigurationRoot configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        /// <summary>
        /// Add authorization policies
        /// </summary>
        /// <param name="services"></param>
        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationConsts.AdministrationPolicy,
                    policy => policy.RequireRole(AuthorizationConsts.AdministrationRole));
            });
        }

        /// <summary>
        /// Add exception filter for controller
        /// </summary>
        /// <param name="services"></param>
        public static void AddMvcExceptionFilters(this IServiceCollection services)
        {
            //Exception handling
            services.AddScoped<ControllerExceptionFilterAttribute>();
        }

        /// <summary>
        /// Register services for authentication, including Identity.
        /// For production mode is used OpenId Connect middleware which is connected to IdentityServer4 instance.
        /// For testing purpose is used cookie middleware with fake login url.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TUserIdentity"></typeparam>
        /// <typeparam name="TUserIdentityRole"></typeparam>
        /// <param name="services"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="adminConfiguration"></param>
        public static void AddAuthenticationServices(this IServiceCollection services, IHostingEnvironment hostingEnvironment, IAdminConfiguration adminConfiguration)
        {
            //services.AddIdentity<TUserIdentity, TUserIdentityRole>(options =>
            //    {
            //        options.User.RequireUniqueEmail = true;
            //    })
            //    .AddEntityFrameworkStores<TContext>()
            //    .AddDefaultTokenProviders();

            //For integration tests use only cookie middleware
            if (hostingEnvironment.IsStaging())
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                        options => { options.Cookie.Name = AuthenticationConsts.IdentityAdminCookieName; });
            }
            else
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = AuthenticationConsts.OidcAuthenticationScheme;

                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                        options =>
                        {
                            options.Cookie.Name = AuthenticationConsts.IdentityAdminCookieName;

                            // Issue: https://github.com/aspnet/Announcements/issues/318
                            options.Cookie.SameSite = SameSiteMode.None;
                        })
                    .AddOpenIdConnect(AuthenticationConsts.OidcAuthenticationScheme, options =>
                    {
                        options.Authority = adminConfiguration.IdentityServerBaseUrl;
#if DEBUG
                        options.RequireHttpsMetadata = false;
#else
                        options.RequireHttpsMetadata = true;
#endif
                        options.ClientId = adminConfiguration.ClientId;
                        options.ClientSecret = adminConfiguration.ClientSecret;
                        options.ResponseType = adminConfiguration.OidcResponseType;

                        options.Scope.Clear();
                        foreach (var scope in adminConfiguration.Scopes)
                        {
                            options.Scope.Add(scope);
                        }

                        options.ClaimActions.MapJsonKey(AuthenticationConsts.RoleClaim, AuthenticationConsts.RoleClaim, AuthenticationConsts.RoleClaim);

                        options.SaveTokens = true;

                        options.GetClaimsFromUserInfoEndpoint = true;

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = JwtClaimTypes.Name,
                            RoleClaimType = JwtClaimTypes.Role,
                        };

                        options.Events = new OpenIdConnectEvents
                        {
                            OnMessageReceived = OnMessageReceived,
                            OnRedirectToIdentityProvider = n => OnRedirectToIdentityProvider(n, adminConfiguration)
                        };
                    });
            }
        }

        /// <summary>
        /// Configuration root configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureRootConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            services.Configure<AdminConfiguration>(configuration.GetSection(ConfigurationConsts.AdminConfigurationKey));

            services.TryAddSingleton<IRootConfiguration, RootConfiguration>();

            return services;
        }

        private static Task OnMessageReceived(MessageReceivedContext context)
        {
            context.Properties.IsPersistent = true;
            context.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddHours(12));

            return Task.FromResult(0);
        }

        private static Task OnRedirectToIdentityProvider(RedirectContext n, IAdminConfiguration adminConfiguration)
        {
            n.ProtocolMessage.RedirectUri = adminConfiguration.IdentityAdminRedirectUri;

            return Task.FromResult(0);
        }
    }
}