using Iserv.IdentityServer4.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.STS.Identity.Helpers.Localization;
using System;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Constants;

namespace Skoruba.IdentityServer4.STS.Identity.Controllers
{
    [ApiController]
    [Authorize(AuthorizationConsts.SealedPolicy)]
    [Route("api")]
    public class SealedController<TUser, TKey> : Controller
        where TUser : UserIdentity<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IGenericControllerLocalizer<ApiController<TUser, TKey>> _localizer;
        private readonly IAccountService<TUser, TKey> _accountService;
        private readonly IPortalService _portalService;

        public SealedController(UserManager<TUser> userManager,
            IGenericControllerLocalizer<ApiController<TUser, TKey>> localizer,
            IAccountService<TUser, TKey> accountService, IPortalService portalService)
        {
            _userManager = userManager;
            _localizer = localizer;
            _accountService = accountService;
            _portalService = portalService;
        }
        
        [HttpPost]
        [Route("getSessionPortal")]
        public IActionResult GetSessionPortal()
        {
            return Ok(_portalService.GetCookie());
        }
    }
}