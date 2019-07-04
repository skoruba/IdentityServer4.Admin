using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Tenant;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.ExceptionHandling;
using System;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.Controllers
{
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    public class TenantController : BaseController
    {
        public TenantController(ITenantService tenantService, ILogger<BaseController> logger) : base(logger)
        {
            TenantService = tenantService;
        }

        public ITenantService TenantService { get; }

        public async Task<IActionResult> Index()
        {
            var tenants = await TenantService.GetTenantsAsync();
            return View(tenants);
        }

        [HttpGet]
        public async Task<IActionResult> TenantProfile(string id)
        {
            if (id == null)
            {
                return View(new TenantDto() { Id = Guid.Empty.ToString() });
            }

            var tenant = await TenantService.GetTenantAsync(id);
            return View(tenant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TenantProfile(TenantDto tenant)
        {
            if (!ModelState.IsValid) return View(tenant);
            if (Equals(tenant.Id, Guid.Empty.ToString()))
            {
                var newTenant = await TenantService.AddTenantAsync(tenant);
                return RedirectToAction(nameof(TenantProfile), newTenant);
            }
            else
            {
                var newTenant = new TenantDto() { Id = Guid.Empty.ToString() };
                return RedirectToAction(nameof(TenantProfile), newTenant);
            }
        }
    }
}