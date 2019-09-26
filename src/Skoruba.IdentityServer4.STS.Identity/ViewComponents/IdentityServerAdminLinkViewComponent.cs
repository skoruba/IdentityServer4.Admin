﻿using Microsoft.AspNetCore.Mvc;
using Skoruba.IdentityServer4.Shared.Configuration.Interfaces;

namespace Skoruba.IdentityServer4.STS.Identity.ViewComponents
{
    public class IdentityServerAdminLinkViewComponent : ViewComponent
    {
        private readonly IRootConfiguration _configuration;

        public IdentityServerAdminLinkViewComponent(IRootConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IViewComponentResult Invoke()
        {
            var identityAdminUrl = _configuration.AdminAppConfiguration.IdentityAdminBaseUrl;

            return View(model: identityAdminUrl);
        }
    }
}