using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Log;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;

namespace Skoruba.IdentityServer4.Admin.Controllers
{
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public class LogController : BaseController
    {
        private readonly ILogService _logService;
        private readonly IAuditLogService _auditLogService;

        public LogController(ILogService logService,
            ILogger<ConfigurationController> logger,
            IAuditLogService auditLogService) : base(logger)
        {
            _logService = logService;
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> ErrorsLog(int? page, string search)
        {
            ViewBag.Search = search;
            var logs = await _logService.GetLogsAsync(search, page ?? 1);

            return View(logs);
        }

        [HttpGet]
        public async Task<IActionResult> AuditLog([FromQuery]AuditLogFilterDto filters)
        {
            ViewBag.SubjectIdentifier = filters.SubjectIdentifier;
            ViewBag.SubjectName = filters.SubjectName;
            ViewBag.Event = filters.Event;
            ViewBag.Source = filters.Source;
            ViewBag.Category = filters.Category;

            var logs = await _auditLogService.GetAsync(filters);

            return View(logs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLogs(LogsDto logs)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(ErrorsLog), logs);
            }
            
            await _logService.DeleteLogsOlderThanAsync(logs.DeleteOlderThan.Value);

            return RedirectToAction(nameof(ErrorsLog));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAuditLogs(AuditLogsDto logs)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(AuditLog), logs);
            }

            await _auditLogService.DeleteLogsOlderThanAsync(logs.DeleteOlderThan.Value);

            return RedirectToAction(nameof(AuditLog));
        }
    }
}