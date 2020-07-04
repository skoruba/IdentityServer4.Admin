using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration.Tenants;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.ExceptionHandling;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.Controllers
{
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    public class EditionController : BaseController
    {
        private readonly IEditionService EditionService;
        public EditionController(
            ILogger<EditionController> logger,
            IEditionService editionService)
             : base(logger)
        {
            EditionService = editionService;
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Index(int? page, string search)
        {
            ViewBag.Search = search;
            var editions = await EditionService.GetListAsync(search, page ?? 1);
            return View(editions);
        }

        [HttpGet]
        [Route("[controller]/[action]/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var edition = await EditionService.GetAsync(id);
            return View(edition);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new EditionDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEditionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            var newEditionDto = await EditionService.CreateAsync(dto);
            SuccessNotification("Saved successfully.", "Success");

            return RedirectToAction(nameof(Update), new { id = newEditionDto.Id });
        }

        [HttpGet]
        [Route("[controller]/[action]/{id}")]
        public async Task<IActionResult> Update(string id)
        {
            var editionDto = await EditionService.GetAsync(id);
            if (editionDto == null) return NotFound();

            return View(editionDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateEditionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            var updatedEditionDto = await EditionService.UpdateAsync(dto);
            SuccessNotification("Updated successfully.", "Success");

            return RedirectToAction(nameof(Update), new { id = updatedEditionDto.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var editionDto = await EditionService.GetAsync(id);
            if (editionDto == null) return NotFound();

            return View(editionDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(EditionDto dto)
        {
            await EditionService.DeleteAsync(dto.Id);
            SuccessNotification("Deleted successfully.", "Success");
            return RedirectToAction(nameof(Index));
        }

    }
}
