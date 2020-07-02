using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration.Tenants;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.Controllers
{
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    public class TenantController : BaseController
    {
        private readonly ITenantService TenantService;
        public TenantController(
            ILogger<TenantController> logger,
            ITenantService tenantService)
             : base(logger)
        {
            TenantService = tenantService;
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Index(int? page, string search)
        {
            ViewBag.Search = search;
            var tenants = await TenantService.GetListAsync(search, page ?? 1);
            return View(tenants);
        }

        [HttpGet]
        [Route("[controller]/[action]/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var role = await TenantService.GetAsync(id);
            return View(role);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new TenantDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTenantDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            var newTenantDto = await TenantService.CreateAsync(dto);
            SuccessNotification("Saved successfully.", "Success");

            return RedirectToAction(nameof(Update), new { id = newTenantDto.Id });
        }

        [HttpGet]
        [Route("[controller]/[action]/{id}")]
        public async Task<IActionResult> Update(string id)
        {
            var tenantDto = await TenantService.GetAsync(id);
            if (tenantDto == null) return NotFound();

            return View(tenantDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateTenantDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            var updatedTenantDto = await TenantService.UpdateAsync(dto);
            SuccessNotification("Updated successfully.", "Success");

            return RedirectToAction(nameof(Update), new { id = updatedTenantDto.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var tenantDto = await TenantService.GetAsync(id);
            if (tenantDto == null) return NotFound();

            return View(tenantDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(TenantDto dto)
        {
            await TenantService.DeleteAsync(dto.Id);
            SuccessNotification("Deleted successfully.", "Success");
            return RedirectToAction(nameof(Index));
        }

    }
}
