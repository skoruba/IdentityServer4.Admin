using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
using SkorubaIdentityServer4Admin.Admin.ExceptionHandling;
using SkorubaIdentityServer4Admin.Admin.Middlewares;
using SkorubaIdentityServer4Admin.Admin.Configuration;
using SkorubaIdentityServer4Admin.Admin.Configuration.Constants;
using SkorubaIdentityServer4Admin.Admin.Configuration.Interfaces;

namespace SkorubaIdentityServer4Admin.Admin.Helpers
{
	public static class StartupHelpers
    {
        public static void RegisterDbContexts<TContext>(this IServiceCollection services, IConfigurationRoot configuration) 
            where TContext : DbContext
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var operationalStoreOptions = new OperationalStoreOptions();
            services.AddSingleton(operationalStoreOptions);

            var storeOptions = new ConfigurationStoreOptions();
            services.AddSingleton(storeOptions);

            services.AddDbContext<TContext>(options => options.UseSqlServer(configuration.GetConnectionString(ConfigurationConsts.AdminConnectionStringKey), optionsSql => optionsSql.MigrationsAssembly(migrationsAssembly)));
        }

        public static void RegisterDbContextsStaging<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            var databaseName = Guid.NewGuid().ToString();

            var operationalStoreOptions = new OperationalStoreOptions();
            services.AddSingleton(operationalStoreOptions);

            var storeOptions = new ConfigurationStoreOptions();
            services.AddSingleton(storeOptions);

            services.AddDbContext<TContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(databaseName));
        }

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

        public static void ConfigureAuthentificationServices(this IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsStaging())
            {
                app.UseMiddleware<AuthenticatedTestRequestMiddleware>();
            }
        }

        public static void ConfigureLocalization(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
        }

        public static void AddLogging(this IApplicationBuilder app, ILoggerFactory loggerFactory, IConfigurationRoot configuration)
        {
	        Log.Logger = new LoggerConfiguration()
	            .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        public static void AddDbContexts<TContext>(this IServiceCollection services, IHostingEnvironment hostingEnvironment, IConfigurationRoot configuration)
        where TContext : DbContext
        {
            if (hostingEnvironment.IsStaging())
            {
                services.RegisterDbContextsStaging<TContext>();
            }
            else
            {
                services.RegisterDbContexts<TContext>(configuration);
            }
        }

        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationConsts.AdministrationPolicy,
                    policy => policy.RequireRole(AuthorizationConsts.AdministrationRole));
            });
        }

        public static void AddMvcExceptionFilters(this IServiceCollection services)
        {
            //Exception handling
            services.AddScoped<ControllerExceptionFilterAttribute>();
        }

        public static void AddMvcLocalization(this IServiceCollection services)
        {
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

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
                        new CultureInfo("zh"),
                        new CultureInfo("en")
                    };

                    opts.DefaultRequestCulture = new RequestCulture("en");
                    opts.SupportedCultures = supportedCultures;
                    opts.SupportedUICultures = supportedCultures;
                });
        }

        public static void AddAuthenticationServices<TContext, TUserIdentity, TUserIdentityRole>(this IServiceCollection services, IHostingEnvironment hostingEnvironment, IAdminConfiguration adminConfiguration)
            where TContext : DbContext where TUserIdentity : class where TUserIdentityRole : class
        {
            services.AddIdentity<TUserIdentity, TUserIdentityRole>()
                .AddEntityFrameworkStores<TContext>()
                .AddDefaultTokenProviders();

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
                        options => { options.Cookie.Name = AuthenticationConsts.IdentityAdminCookieName; })
                    .AddOpenIdConnect(AuthenticationConsts.OidcAuthenticationScheme, options =>
                    {
                        options.Authority = adminConfiguration.IdentityServerBaseUrl;
                        options.RequireHttpsMetadata = false;

                        options.ClientId = AuthenticationConsts.OidcClientId;

                        options.Scope.Clear();
                        options.Scope.Add(AuthenticationConsts.ScopeOpenId);
                        options.Scope.Add(AuthenticationConsts.ScopeProfile);
                        options.Scope.Add(AuthenticationConsts.ScopeEmail);
                        options.Scope.Add(AuthenticationConsts.ScopeRoles);

                        options.SaveTokens = true;

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

        public static IServiceCollection ConfigureRootConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            services.Configure<AdminConfiguration>(configuration.GetSection(ConfigurationConsts.AdminConfigurationKey));

            services.TryAddSingleton<IRootConfiguration, RootConfiguration>();

            return services;
        }
    }
}
