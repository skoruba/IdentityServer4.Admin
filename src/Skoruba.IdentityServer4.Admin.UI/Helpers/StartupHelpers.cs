using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Skoruba.AuditLogging.EntityFramework.DbContexts;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.AuditLogging.EntityFramework.Extensions;
using Skoruba.AuditLogging.EntityFramework.Repositories;
using Skoruba.AuditLogging.EntityFramework.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.UI.Configuration;
using Skoruba.IdentityServer4.Admin.UI.Configuration.ApplicationParts;
using Skoruba.IdentityServer4.Admin.UI.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Helpers;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.UI.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.UI.Helpers.Localization;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.HttpOverrides;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Hosting;
using Skoruba.IdentityServer4.Admin.UI.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Skoruba.IdentityServer4.Admin.EntityFramework.Configuration.Configuration;
using Skoruba.IdentityServer4.Admin.EntityFramework.Configuration.MySql;
using Skoruba.IdentityServer4.Admin.EntityFramework.Configuration.PostgreSQL;
using Skoruba.IdentityServer4.Admin.EntityFramework.Configuration.SqlServer;
using Skoruba.IdentityServer4.Shared.Configuration.Authentication;

namespace Skoruba.IdentityServer4.Admin.UI.Helpers
{
    public static class StartupHelpers
    {
        /// <summary>
        /// A helper to inject common middleware into the application pipeline, without having to invoke Use*() methods.
        /// </summary>
        internal class StartupFilter : IStartupFilter
        {
            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            {
                return builder =>
                {
                    // Adds our required middlewares to the beginning of the app pipeline.
                    // This does not include the middleware that is required to go between UseRouting and UseEndpoints.
                    builder.UseCommonMiddleware(
                        builder.ApplicationServices.GetRequiredService<SecurityConfiguration>(),
                        builder.ApplicationServices.GetRequiredService<HttpConfiguration>());

                    next(builder);

                    // Routing-dependent middleware needs to go in between UseRouting and UseEndpoints and therefore 
                    // needs to be handled by the user using UseIdentityServer4AdminUI().
                };
            }
        }

        public static IServiceCollection AddAuditEventLogging<TAuditLoggingDbContext, TAuditLog>(this IServiceCollection services, AuditLoggingConfiguration auditLoggingConfiguration)
            where TAuditLog : AuditLog, new()
            where TAuditLoggingDbContext : IAuditLoggingDbContext<TAuditLog>
        {
            services.AddAuditLogging(options => { options.Source = auditLoggingConfiguration.Source; })
                .AddDefaultHttpEventData(subjectOptions =>
                {
                    subjectOptions.SubjectIdentifierClaim = auditLoggingConfiguration.SubjectIdentifierClaim;
                    subjectOptions.SubjectNameClaim = auditLoggingConfiguration.SubjectNameClaim;
                },
                    actionOptions =>
                    {
                        actionOptions.IncludeFormVariables = auditLoggingConfiguration.IncludeFormVariables;
                    })
                .AddAuditSinks<DatabaseAuditEventLoggerSink<TAuditLog>>();

            // repository for library
            services.AddTransient<IAuditLoggingRepository<TAuditLog>, AuditLoggingRepository<TAuditLoggingDbContext, TAuditLog>>();

            // repository and service for admin
            services.AddTransient<IAuditLogRepository<TAuditLog>, AuditLogRepository<TAuditLoggingDbContext, TAuditLog>>();
            services.AddTransient<IAuditLogService, AuditLogService<TAuditLog>>();

            return services;
        }

        /// <summary>
        /// Register DbContexts for IdentityServer ConfigurationStore and PersistedGrants, Identity and Logging
        /// Configure the connection strings in AppSettings.json
        /// </summary>
        /// <typeparam name="TConfigurationDbContext"></typeparam>
        /// <typeparam name="TPersistedGrantDbContext"></typeparam>
        /// <typeparam name="TLogDbContext"></typeparam>
        /// <typeparam name="TIdentityDbContext"></typeparam>
        /// <typeparam name="TAuditLoggingDbContext"></typeparam>
        /// <typeparam name="TDataProtectionDbContext"></typeparam>
        /// <typeparam name="TAuditLog"></typeparam>
        /// <param name="services"></param>
        /// <param name="connectionStrings"></param>
        /// <param name="databaseProvider"></param>
        /// <param name="databaseMigrations"></param>
        public static void RegisterDbContexts<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext,
            TLogDbContext, TAuditLoggingDbContext, TDataProtectionDbContext, TAuditLog>(
            this IServiceCollection services,
            ConnectionStringsConfiguration connectionStrings,
            DatabaseProviderConfiguration databaseProvider,
            DatabaseMigrationsConfiguration databaseMigrations)
            where TIdentityDbContext : DbContext
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
            where TAuditLoggingDbContext : DbContext, IAuditLoggingDbContext<TAuditLog>
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
            where TAuditLog : AuditLog
        {
            switch (databaseProvider.ProviderType)
            {
                case DatabaseProviderType.SqlServer:
                    services.RegisterSqlServerDbContexts<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLoggingDbContext, TDataProtectionDbContext, TAuditLog>(connectionStrings, databaseMigrations);
                    break;
                case DatabaseProviderType.PostgreSQL:
                    services.RegisterNpgSqlDbContexts<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLoggingDbContext, TDataProtectionDbContext, TAuditLog>(connectionStrings, databaseMigrations);
                    break;
                case DatabaseProviderType.MySql:
                    services.RegisterMySqlDbContexts<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLoggingDbContext, TDataProtectionDbContext, TAuditLog>(connectionStrings, databaseMigrations);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseProvider.ProviderType), $@"The value needs to be one of {string.Join(", ", Enum.GetNames(typeof(DatabaseProviderType)))}.");
            }
        }

        /// <summary>
        /// Register in memory DbContexts for IdentityServer ConfigurationStore and PersistedGrants, Identity and Logging
        /// For testing purpose only
        /// </summary>
        /// <typeparam name="TConfigurationDbContext"></typeparam>
        /// <typeparam name="TPersistedGrantDbContext"></typeparam>
        /// <typeparam name="TLogDbContext"></typeparam>
        /// <typeparam name="TIdentityDbContext"></typeparam>
        /// <typeparam name="TAuditLoggingDbContext"></typeparam>
        /// <typeparam name="TDataProtectionDbContext"></typeparam>
        /// <param name="services"></param>
        public static void RegisterDbContextsStaging<TIdentityDbContext, TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext, TAuditLoggingDbContext, TDataProtectionDbContext, TAuditLog>(this IServiceCollection services)
            where TIdentityDbContext : DbContext
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
            where TAuditLoggingDbContext : DbContext, IAuditLoggingDbContext<TAuditLog>
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
            where TAuditLog : AuditLog
        {
            var persistedGrantsDatabaseName = Guid.NewGuid().ToString();
            var configurationDatabaseName = Guid.NewGuid().ToString();
            var logDatabaseName = Guid.NewGuid().ToString();
            var identityDatabaseName = Guid.NewGuid().ToString();
            var auditLoggingDatabaseName = Guid.NewGuid().ToString();
            var dataProtectionDatabaseName = Guid.NewGuid().ToString();

            var operationalStoreOptions = new OperationalStoreOptions();
            services.AddSingleton(operationalStoreOptions);

            var storeOptions = new ConfigurationStoreOptions();
            services.AddSingleton(storeOptions);

            services.AddDbContext<TIdentityDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(identityDatabaseName));
            services.AddDbContext<TPersistedGrantDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(persistedGrantsDatabaseName));
            services.AddDbContext<TConfigurationDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(configurationDatabaseName));
            services.AddDbContext<TLogDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(logDatabaseName));
            services.AddDbContext<TAuditLoggingDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(auditLoggingDatabaseName));
            services.AddDbContext<TDataProtectionDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(dataProtectionDatabaseName));
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
        /// Add authorization policies
        /// </summary>
        /// <param name="services"></param>
        public static void AddAuthorizationPolicies(this IServiceCollection services, AdminConfiguration adminConfiguration,
            Action<AuthorizationOptions> authorizationAction)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationConsts.AdministrationPolicy,
                    policy => policy.RequireRole(adminConfiguration.AdministrationRole));

                authorizationAction?.Invoke(options);
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
        /// Register services for MVC and localization including available languages
        /// </summary>
        /// <param name="services"></param>
        public static void AddMvcWithLocalization<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>
            (this IServiceCollection services, CultureConfiguration cultureConfiguration)
            where TUserDto : UserDto<TKey>, new()
            where TRoleDto : RoleDto<TKey>, new()
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>
            where TUserRole : IdentityUserRole<TKey>
            where TUserLogin : IdentityUserLogin<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>
            where TUserToken : IdentityUserToken<TKey>
            where TUsersDto : UsersDto<TUserDto, TKey>
            where TRolesDto : RolesDto<TRoleDto, TKey>
            where TUserRolesDto : UserRolesDto<TRoleDto, TKey>
            where TUserClaimsDto : UserClaimsDto<TUserClaimDto, TKey>
            where TUserProviderDto : UserProviderDto<TKey>
            where TUserProvidersDto : UserProvidersDto<TUserProviderDto, TKey>
            where TUserChangePasswordDto : UserChangePasswordDto<TKey>
            where TRoleClaimsDto : RoleClaimsDto<TRoleClaimDto, TKey>
            where TUserClaimDto : UserClaimDto<TKey>
            where TRoleClaimDto : RoleClaimDto<TKey>
        {
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            services.AddLocalization(opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; });

            services.TryAddTransient(typeof(IGenericControllerLocalizer<>), typeof(GenericControllerLocalizer<>));

            services.AddTransient<IViewLocalizer, ResourceViewLocalizer>();

            services.AddControllersWithViews(o =>
                {
                    o.Conventions.Add(new GenericControllerRouteConvention());
                })
#if DEBUG
                // It is not necessary to re-build the views for new changes
                .AddRazorRuntimeCompilation()
#endif
                .AddViewLocalization(
                    LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; })
                .AddDataAnnotationsLocalization()
                .ConfigureApplicationPartManager(m =>
                {
                    m.FeatureProviders.Add(new GenericTypeControllerFeatureProvider<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>());
                });

            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    // If cultures are specified in the configuration, use them (making sure they are among the available cultures),
                    // otherwise use all the available cultures
                    var supportedCultureCodes = (cultureConfiguration?.Cultures?.Count > 0 ?
                        cultureConfiguration.Cultures.Intersect(CultureConfiguration.AvailableCultures) :
                        CultureConfiguration.AvailableCultures).ToArray();

                    if (!supportedCultureCodes.Any()) supportedCultureCodes = CultureConfiguration.AvailableCultures;
                    var supportedCultures = supportedCultureCodes.Select(c => new CultureInfo(c)).ToList();

                    // If the default culture is specified use it, otherwise use CultureConfiguration.DefaultRequestCulture ("en")
                    var defaultCultureCode = string.IsNullOrEmpty(cultureConfiguration?.DefaultCulture) ?
                        CultureConfiguration.DefaultRequestCulture : cultureConfiguration?.DefaultCulture;

                    // If the default culture is not among the supported cultures, use the first supported culture as default
                    if (!supportedCultureCodes.Contains(defaultCultureCode)) defaultCultureCode = supportedCultureCodes.FirstOrDefault();

                    opts.DefaultRequestCulture = new RequestCulture(defaultCultureCode);
                    opts.SupportedCultures = supportedCultures;
                    opts.SupportedUICultures = supportedCultures;
                });
        }

        public static void AddAuthenticationServicesStaging<TContext, TUserIdentity, TUserIdentityRole>(
            this IServiceCollection services)
            where TContext : DbContext where TUserIdentity : class where TUserIdentityRole : class
        {
            services.AddIdentity<TUserIdentity, TUserIdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<TContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
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
        /// <param name="adminConfiguration"></param>
        public static void AddAuthenticationServices<TContext, TUserIdentity, TUserIdentityRole>(this IServiceCollection services,
            AdminConfiguration adminConfiguration, Action<IdentityOptions> identityOptionsAction, Action<AuthenticationBuilder> authenticationBuilderAction)
            where TContext : DbContext where TUserIdentity : class where TUserIdentityRole : class
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.Secure = CookieSecurePolicy.SameAsRequest;
                options.OnAppendCookie = cookieContext =>
                    AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            services
                .AddIdentity<TUserIdentity, TUserIdentityRole>(identityOptionsAction)
                .AddEntityFrameworkStores<TContext>()
                .AddDefaultTokenProviders();

            var authenticationBuilder = services.AddAuthentication(options =>
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
                            options.Cookie.Name = adminConfiguration.IdentityAdminCookieName;
                        })
                    .AddOpenIdConnect(AuthenticationConsts.OidcAuthenticationScheme, options =>
                    {
                        options.Authority = adminConfiguration.IdentityServerBaseUrl;
                        options.RequireHttpsMetadata = adminConfiguration.RequireHttpsMetadata;
                        options.ClientId = adminConfiguration.ClientId;
                        options.ClientSecret = adminConfiguration.ClientSecret;
                        options.ResponseType = adminConfiguration.OidcResponseType;

                        options.Scope.Clear();
                        foreach (var scope in adminConfiguration.Scopes)
                        {
                            options.Scope.Add(scope);
                        }

                        options.ClaimActions.MapJsonKey(adminConfiguration.TokenValidationClaimRole, adminConfiguration.TokenValidationClaimRole, adminConfiguration.TokenValidationClaimRole);

                        options.SaveTokens = true;

                        options.GetClaimsFromUserInfoEndpoint = true;

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = adminConfiguration.TokenValidationClaimName,
                            RoleClaimType = adminConfiguration.TokenValidationClaimRole
                        };

                        options.Events = new OpenIdConnectEvents
                        {
                            OnMessageReceived = context => OnMessageReceived(context, adminConfiguration),
                            OnRedirectToIdentityProvider = context => OnRedirectToIdentityProvider(context, adminConfiguration)
                        };
                    });

            authenticationBuilderAction?.Invoke(authenticationBuilder);
        }


        private static Task OnMessageReceived(MessageReceivedContext context, AdminConfiguration adminConfiguration)
        {
            context.Properties.IsPersistent = true;
            context.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddHours(adminConfiguration.IdentityAdminCookieExpiresUtcHours));

            return Task.FromResult(0);
        }

        private static Task OnRedirectToIdentityProvider(RedirectContext n, AdminConfiguration adminConfiguration)
        {
            n.ProtocolMessage.RedirectUri = adminConfiguration.IdentityAdminRedirectUri;

            return Task.FromResult(0);
        }

        public static void AddIdSHealthChecks<TConfigurationDbContext, TPersistedGrantDbContext, TIdentityDbContext,
            TLogDbContext, TAuditLoggingDbContext, TDataProtectionDbContext, TAuditLog>
            (this IHealthChecksBuilder healthChecksBuilder, AdminConfiguration adminConfiguration,
            ConnectionStringsConfiguration connectionStringsConfiguration, DatabaseProviderConfiguration databaseProviderConfiguration)
            where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TIdentityDbContext : DbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
            where TAuditLoggingDbContext : DbContext, IAuditLoggingDbContext<TAuditLog>
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
            where TAuditLog : AuditLog
        {
            var configurationDbConnectionString = connectionStringsConfiguration.ConfigurationDbConnection;
            var persistedGrantsDbConnectionString = connectionStringsConfiguration.PersistedGrantDbConnection;
            var identityDbConnectionString = connectionStringsConfiguration.IdentityDbConnection;
            var logDbConnectionString = connectionStringsConfiguration.AdminLogDbConnection;
            var auditLogDbConnectionString = connectionStringsConfiguration.AdminAuditLogDbConnection;
            var dataProtectionDbConnectionString = connectionStringsConfiguration.DataProtectionDbConnection;

            var identityServerUri = adminConfiguration.IdentityServerBaseUrl;
            healthChecksBuilder = healthChecksBuilder
                .AddDbContextCheck<TConfigurationDbContext>("ConfigurationDbContext")
                .AddDbContextCheck<TPersistedGrantDbContext>("PersistedGrantsDbContext")
                .AddDbContextCheck<TIdentityDbContext>("IdentityDbContext")
                .AddDbContextCheck<TLogDbContext>("LogDbContext")
                .AddDbContextCheck<TAuditLoggingDbContext>("AuditLogDbContext")
                .AddDbContextCheck<TDataProtectionDbContext>("DataProtectionDbContext")

                .AddIdentityServer(new Uri(identityServerUri), "Identity Server");

            var serviceProvider = healthChecksBuilder.Services.BuildServiceProvider();
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var configurationTableName = DbContextHelpers.GetEntityTable<TConfigurationDbContext>(scope.ServiceProvider);
                var persistedGrantTableName = DbContextHelpers.GetEntityTable<TPersistedGrantDbContext>(scope.ServiceProvider);
                var identityTableName = DbContextHelpers.GetEntityTable<TIdentityDbContext>(scope.ServiceProvider);
                var logTableName = DbContextHelpers.GetEntityTable<TLogDbContext>(scope.ServiceProvider);
                var auditLogTableName = DbContextHelpers.GetEntityTable<TAuditLoggingDbContext>(scope.ServiceProvider);
                var dataProtectionTableName = DbContextHelpers.GetEntityTable<TDataProtectionDbContext>(scope.ServiceProvider);

                switch (databaseProviderConfiguration.ProviderType)
                {
                    case DatabaseProviderType.SqlServer:
                        healthChecksBuilder
                            .AddSqlServer(configurationDbConnectionString, name: "ConfigurationDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{configurationTableName}]")
                            .AddSqlServer(persistedGrantsDbConnectionString, name: "PersistentGrantsDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{persistedGrantTableName}]")
                            .AddSqlServer(identityDbConnectionString, name: "IdentityDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{identityTableName}]")
                            .AddSqlServer(logDbConnectionString, name: "LogDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{logTableName}]")
                            .AddSqlServer(auditLogDbConnectionString, name: "AuditLogDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{auditLogTableName}]")
                            .AddSqlServer(dataProtectionDbConnectionString, name: "DataProtectionDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{dataProtectionTableName}]");
                        break;
                    case DatabaseProviderType.PostgreSQL:
                        healthChecksBuilder
                            .AddNpgSql(configurationDbConnectionString, name: "ConfigurationDb",
                                healthQuery: $"SELECT * FROM \"{configurationTableName}\" LIMIT 1")
                            .AddNpgSql(persistedGrantsDbConnectionString, name: "PersistentGrantsDb",
                                healthQuery: $"SELECT * FROM \"{persistedGrantTableName}\" LIMIT 1")
                            .AddNpgSql(identityDbConnectionString, name: "IdentityDb",
                                healthQuery: $"SELECT * FROM \"{identityTableName}\" LIMIT 1")
                            .AddNpgSql(logDbConnectionString, name: "LogDb",
                                healthQuery: $"SELECT * FROM \"{logTableName}\" LIMIT 1")
                            .AddNpgSql(auditLogDbConnectionString, name: "AuditLogDb",
                                healthQuery: $"SELECT * FROM \"{auditLogTableName}\"  LIMIT 1")
                            .AddNpgSql(dataProtectionDbConnectionString, name: "DataProtectionDb",
                                healthQuery: $"SELECT * FROM \"{dataProtectionTableName}\"  LIMIT 1");
                        break;
                    case DatabaseProviderType.MySql:
                        healthChecksBuilder
                            .AddMySql(configurationDbConnectionString, name: "ConfigurationDb")
                            .AddMySql(persistedGrantsDbConnectionString, name: "PersistentGrantsDb")
                            .AddMySql(identityDbConnectionString, name: "IdentityDb")
                            .AddMySql(logDbConnectionString, name: "LogDb")
                            .AddMySql(auditLogDbConnectionString, name: "AuditLogDb")
                            .AddMySql(dataProtectionDbConnectionString, name: "DataProtectionDb");
                        break;
                    default:
                        throw new NotImplementedException($"Health checks not defined for database provider {databaseProviderConfiguration.ProviderType}");
                }
            }
        }

        /// <summary>
        /// Using of Forwarded Headers, Hsts, XXssProtection and Csp
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        public static void UseSecurityHeaders(this IApplicationBuilder app, List<string> cspTrustedDomains)
        {
            var forwardingOptions = new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.All
            };

            forwardingOptions.KnownNetworks.Clear();
            forwardingOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(forwardingOptions);

            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXContentTypeOptions();
            app.UseXfo(options => options.SameOrigin());
            app.UseReferrerPolicy(options => options.NoReferrer());

            // CSP Configuration to be able to use external resources
            if (cspTrustedDomains != null && cspTrustedDomains.Any())
            {
                app.UseCsp(csp =>
                {
                    var imagesCustomSources = new List<string>();
                    imagesCustomSources.AddRange(cspTrustedDomains);
                    imagesCustomSources.Add("data:");

                    csp.ImageSources(options =>
                    {
                        options.SelfSrc = true;
                        options.CustomSources = imagesCustomSources;
                        options.Enabled = true;
                    });
                    csp.FontSources(options =>
                    {
                        options.SelfSrc = true;
                        options.CustomSources = cspTrustedDomains;
                        options.Enabled = true;
                    });
                    csp.ScriptSources(options =>
                    {
                        options.SelfSrc = true;
                        options.CustomSources = cspTrustedDomains;
                        options.Enabled = true;
                        options.UnsafeInlineSrc = true;
                        options.UnsafeEvalSrc = true;
                    });
                    csp.StyleSources(options =>
                    {
                        options.SelfSrc = true;
                        options.CustomSources = cspTrustedDomains;
                        options.Enabled = true;
                        options.UnsafeInlineSrc = true;
                    });
                    csp.DefaultSources(options =>
                    {
                        options.SelfSrc = true;
                        options.CustomSources = cspTrustedDomains;
                        options.Enabled = true;
                    });
                });
            }
        }

        public static void UseCommonMiddleware(this IApplicationBuilder app, SecurityConfiguration securityConfiguration, HttpConfiguration httpConfiguration)
        {
            app.UseCookiePolicy();

            if (securityConfiguration.UseDeveloperExceptionPage)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            if (securityConfiguration.UseHsts)
            {
                app.UseHsts();
            }

            app.UsePathBase(httpConfiguration.BasePath);

            // Add custom security headers
            app.UseSecurityHeaders(securityConfiguration.CspTrustedDomains);

            app.UseStaticFiles();

            // Use Localization
            app.ConfigureLocalization();
        }

        public static void UseRoutingDependentMiddleware(this IApplicationBuilder app, TestingConfiguration testingConfiguration)
        {
            app.UseAuthentication();
            if (testingConfiguration.IsStaging)
            {
                app.UseMiddleware<AuthenticatedTestRequestMiddleware>();
            }

            app.UseAuthorization();
        }
    }
}
