using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.ExceptionHandling;

namespace Skoruba.IdentityServer4.Admin.Controllers
{
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    public class ExportController : BaseController
    {
        private readonly IExportService _exportService;
        private readonly IExportIdentityService _exportIdentityService;

        public ExportController(IExportService exportService,
            IExportIdentityService exportIdentityService,
            ILogger<ConfigurationController> logger) : base(logger)
        {
            _exportIdentityService = exportIdentityService;
            _exportService = exportService;
        }

        [HttpGet]
        public async Task<IActionResult> ExportConfig()
        {
            var stream = new MemoryStream(await _exportService.GetExportBytesConfigAsync());
            return new FileStreamResult(stream, "application/....")
            {
                FileDownloadName = $"config_{DateTime.Now.ToString("d")}.json"
            };
        }

        [HttpPost]
        public async Task<IActionResult> ImportConfig(IFormFile file)
        {
            if (file == null)
            {
                return new EmptyResult();
            }
            if (Path.GetExtension(file.FileName) != ".json")
            {
                return BadRequest("Invalid file extension");
            }
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                await _exportService.ImportConfigAsync(await reader.ReadToEndAsync());
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ImportUsers(IFormFile file)
        {
            if (file == null)
            {
                return new EmptyResult();
            }
            if (Path.GetExtension(file.FileName) != ".json")
            {
                return BadRequest("Invalid file extension");
            }
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                await _exportIdentityService.ImportUsersAsync(await reader.ReadToEndAsync());
            }
            return Ok();
        }
    }
}
