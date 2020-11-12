using Microsoft.AspNetCore.Mvc;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;

namespace Skoruba.IdentityServer4.Admin.ViewComponents
{
    public class IdentityServerLinkViewComponent : ViewComponent
    {
        private readonly IRootConfiguration _configuration;

        public IdentityServerLinkViewComponent(IRootConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IViewComponentResult Invoke()
        {
            // EZY-modification (EZYC-3029): allow external url when run in docker
            var identityServerUrl = _configuration.AdminConfiguration.IdentityServerUseExternalBaseUrl ?
                _configuration.AdminConfiguration.IdentityServerExternalBaseUrl:
                _configuration.AdminConfiguration.IdentityServerBaseUrl;

            
            return View(model: identityServerUrl);
        }
    }
}
