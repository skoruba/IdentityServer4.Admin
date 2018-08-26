using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Skoruba.IdentityServer4.Admin.Common.Settings;
using Skoruba.IdentityServer4.Admin.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.Setup
{
    public class AuthenticationSetup
    {
        private readonly IServiceCollection _services;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IAdminAppSettings _settings;

        public AuthenticationSetup(IServiceCollection services, IHostingEnvironment hostingEnvironment, IAdminAppSettings settings)
        {
            _services = services;
            _hostingEnvironment = hostingEnvironment;
            _settings = settings;
        }

        public  void AddAuthentication()
        {
            _services.AddIdentity<UserIdentity, UserIdentityRole>()
                .AddEntityFrameworkStores<AdminDbContext>()
                .AddDefaultTokenProviders();

            //For integration tests use only cookie middleware
            if (_hostingEnvironment.IsStaging())
            {
                _services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                        options => { options.Cookie.Name = _settings.IdentityAdminCookieName; });
            }
            else
            {
                _services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = AuthorizationConsts.OidcAuthenticationScheme;

                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                        options => { options.Cookie.Name = _settings.IdentityAdminCookieName; })
                    .AddOpenIdConnect(AuthorizationConsts.OidcAuthenticationScheme, options =>
                    {
                        options.Authority = _settings.IdentityServerBaseUrl;
                        options.RequireHttpsMetadata = false;

                        options.ClientId = _settings.OidcClientId;

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

        private  Task OnMessageReceived(MessageReceivedContext context)
        {
            context.Properties.IsPersistent = true;
            context.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddHours(12));

            return Task.FromResult(0);
        }

        private  Task OnRedirectToIdentityProvider(RedirectContext n)
        {
            n.ProtocolMessage.RedirectUri = _settings.IdentityAdminRedirectUri;

            return Task.FromResult(0);
        }

    }
}
