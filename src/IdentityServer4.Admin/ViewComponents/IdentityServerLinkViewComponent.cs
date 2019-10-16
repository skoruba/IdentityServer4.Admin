using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Shared.Configuration.Interfaces;

namespace IdentityServer4.Admin.ViewComponents
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
            var identityServerUrl = _configuration.AdminAppConfiguration.IdentityServerBaseUrl;
            
            return View(model: identityServerUrl);
        }
    }
}
