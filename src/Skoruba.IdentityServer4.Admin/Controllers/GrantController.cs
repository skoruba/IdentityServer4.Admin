using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Grant;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.Helpers;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;

namespace Skoruba.IdentityServer4.Admin.Controllers
{
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    public class GrantController : BaseController
    {
        private readonly IPersistedGrantAspNetIdentityService _persistedGrantService;
        private readonly IStringLocalizer<GrantController> _localizer;

        public GrantController(IPersistedGrantAspNetIdentityService persistedGrantService,
            ILogger<ConfigurationController> logger,
            IStringLocalizer<GrantController> localizer) : base(logger)
        {
            _persistedGrantService = persistedGrantService;
            _localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> PersistedGrants(int? page, string search)
        {
            ViewBag.Search = search;
            var persistedGrants = await _persistedGrantService.GetPersistedGrantsByUsersAsync(search, page ?? 1);

            return View(persistedGrants);
        }

        [HttpGet]
        public async Task<IActionResult> PersistedGrantDelete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var grant = await _persistedGrantService.GetPersistedGrantAsync(UrlHelpers.QueryStringUnSafeHash(id));
            if (grant == null) return NotFound();

            return View(grant);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersistedGrantDelete(PersistedGrantDto grant)
        {
            await _persistedGrantService.DeletePersistedGrantAsync(grant.Key);

            SuccessNotification(_localizer["SuccessPersistedGrantDelete"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(PersistedGrants));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersistedGrantsDelete(PersistedGrantsDto grants)
        {
            await _persistedGrantService.DeletePersistedGrantsAsync(grants.SubjectId);

            SuccessNotification(_localizer["SuccessPersistedGrantsDelete"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(PersistedGrants));
        }


        [HttpGet]
        public async Task<IActionResult> PersistedGrant(string id, int? page)
        {
            var persistedGrants = await _persistedGrantService.GetPersistedGrantsByUserAsync(id, page ?? 1);
            persistedGrants.SubjectId = id;

            return View(persistedGrants);
        }
    }
}