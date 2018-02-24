using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Skoruba.IdentityServer4.Admin.Configuration;
using Skoruba.IdentityServer4.Admin.Constants;
using Skoruba.IdentityServer4.Admin.Data.DbContexts;
using Skoruba.IdentityServer4.Admin.Data.Entities.Identity;
using Skoruba.IdentityServer4.Admin.Data.Repositories;
using Skoruba.IdentityServer4.Admin.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.Helpers;
using Skoruba.IdentityServer4.Admin.Middlewares;
using Skoruba.IdentityServer4.Admin.Services;

namespace Skoruba.IdentityServer4.Admin
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var builder = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();

            HostingEnvironment = env;
        }

        public IConfigurationRoot Configuration { get; }

        public IHostingEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            AddDbContexts(services);
            AddAuthentication(services);
            AddServices(services);
            AddMvcLocalization(services);
            AddAuthorization(services);
        }

        private void AddDbContexts(IServiceCollection services)
        {
            if (HostingEnvironment.IsStaging())
            {
                services.RegisterDbContextsStaging();
            }
            else
            {
                services.RegisterDbContexts(Configuration);
            }
        }

        private void AddAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationConsts.AdministrationPolicy,
                    policy => policy.RequireRole(AuthorizationConsts.AdministrationRole));
            });
        }

        private void AddServices(IServiceCollection services)
        {
            //Repositories
            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IIdentityResourceRepository, IdentityResourceRepository>();
            services.AddTransient<IIdentityRepository, IdentityRepository>();
            services.AddTransient<IApiResourceRepository, ApiResourceRepository>();
            services.AddTransient<IPersistedGrantRepository, PersistedGrantRepository>();
            services.AddTransient<ILogRepository, LogRepository>();

            //Services
            services.AddTransient<ILogService, LogService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IApiResourceService, ApiResourceService>();
            services.AddTransient<IIdentityResourceService, IdentityResourceService>();
            services.AddTransient<IPersistedGrantService, PersistedGrantService>();
            services.AddTransient<IIdentityService, IdentityService>();

            //Exception handling
            services.AddScoped<ControllerExceptionFilterAttribute>();
        }

        private void AddMvcLocalization(IServiceCollection services)
        {
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            services.AddLocalization(opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; });

            services.AddMvc()
                .AddViewLocalization(
                    LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; })
                .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supportedCultures = new[]
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("en")
                    };

                    opts.DefaultRequestCulture = new RequestCulture("en");
                    opts.SupportedCultures = supportedCultures;
                    opts.SupportedUICultures = supportedCultures;
                });
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services.AddIdentity<UserIdentity, UserIdentityRole>()
                .AddEntityFrameworkStores<AdminDbContext>()
                .AddDefaultTokenProviders();

            //For integration tests use only cookie middleware
            if (HostingEnvironment.IsStaging())
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
                        options => { options.Cookie.Name = AuthorizationConsts.IdentityAdminCookieName; });
            }
            else
            {
                services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = AuthorizationConsts.OidcAuthenticationScheme;

                        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    })
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                        options => { options.Cookie.Name = AuthorizationConsts.IdentityAdminCookieName; })
                    .AddOpenIdConnect(AuthorizationConsts.OidcAuthenticationScheme, options =>
                    {
                        options.Authority = AuthorizationConsts.IdentityServerBaseUrl;
                        options.RequireHttpsMetadata = false;

                        options.ClientId = AuthorizationConsts.OidcClientId;

                        options.Scope.Clear();
                        options.Scope.Add(AuthorizationConsts.ScopeOpenId);
                        options.Scope.Add(AuthorizationConsts.ScopeProfile);
                        options.Scope.Add(AuthorizationConsts.ScopeEmail);
                        options.Scope.Add(AuthorizationConsts.ScopeRoles);

                        options.SaveTokens = true;

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = JwtClaimTypes.Name,
                            RoleClaimType = JwtClaimTypes.Role,
                        };

                        options.Events = new OpenIdConnectEvents
                        {
                            OnMessageReceived = OnMessageReceived,
                            OnRedirectToIdentityProvider = OnRedirectToIdentityProvider
                        };
                    });
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            AddLogging(loggerFactory);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Setup Databases
            //MigrateDbContexts(app);

            AddSecurityHeaders(app);

            app.UseStaticFiles();

            ConfigureAuthentification(app, env);
            ConfigureLocalization(app);

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void AddSecurityHeaders(IApplicationBuilder app)
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

        private void AddLogging(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection(ConfigurationConsts.LoggingSectionKey));
            loggerFactory.AddDebug();

            var columnOptions = new ColumnOptions();

            // Don't include the Properties XML column.
            columnOptions.Store.Remove(StandardColumn.Properties);

            // Do include the log event data as JSON.
            columnOptions.Store.Add(StandardColumn.LogEvent);
            
            Log.Logger = new LoggerConfiguration()
                .WriteTo.MSSqlServer(Configuration.GetConnectionString(ConfigurationConsts.AdminConnectionStringKey),
                    ConfigurationConsts.LoggingTableName,
                    columnOptions: columnOptions,
                    restrictedToMinimumLevel: LogEventLevel.Error)
                .CreateLogger();
        }

        private void ConfigureLocalization(IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
        }

        private void MigrateDbContexts(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<AdminDbContext>().Database.Migrate();
                EnsureSeedData(serviceScope.ServiceProvider.GetService<AdminDbContext>());
            }
        }

        private void ConfigureAuthentification(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsStaging())
            {
                app.UseMiddleware<AuthenticatedTestRequestMiddleware>();
            }
        }

        private Task OnMessageReceived(MessageReceivedContext context)
        {
            context.Properties.IsPersistent = true;
            context.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddHours(12));

            return Task.FromResult(0);
        }

        private Task OnRedirectToIdentityProvider(RedirectContext n)
        {
            n.ProtocolMessage.RedirectUri = AuthorizationConsts.IdentityAdminRedirectUri;

            return Task.FromResult(0);
        }

        private void EnsureSeedData(AdminDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in Clients.GetAdminClient().ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                var identityResources = ClientResources.GetIdentityResources().ToList();

                foreach (var resource in identityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in ClientResources.GetApiResources().ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
        }
    }
}