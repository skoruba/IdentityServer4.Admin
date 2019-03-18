using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.Configuration;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;

namespace Skoruba.IdentityServer4.Admin.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly RootConfiguration _rootConfiguration;

        public AccountController(ILogger<ConfigurationController> logger, RootConfiguration rootConfiguration) : base(logger)
        {
            _rootConfiguration = rootConfiguration;
        }

        public IActionResult AccessDenied()
        {
            ViewData["IdentityServerBaseUrl"] = _rootConfiguration.AdminConfiguration.IdentityServerBaseUrl;

            return View();
        }

        public IActionResult Logout()
        {
            return new SignOutResult(new List<string> { AuthenticationConsts.SignInScheme, AuthenticationConsts.OidcAuthenticationScheme },
                new AuthenticationProperties { RedirectUri = "/" });
        }
    }
}
