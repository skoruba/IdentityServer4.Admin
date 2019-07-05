using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.ViewModels.Audit;
using Skoruba.IdentityServer4.Audit.Core.Interfaces;
using Skoruba.IdentityServer4.Audit.Core.Query;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.Controllers
{
    // TODO: figure out how to call the api
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    public class AuditController : BaseController
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService, ILogger<AuditController> logger) : base(logger)
        {
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var query = new GetAudits() { Page = 1, PageSize = 25 };
            var result = await _auditService.GetAuditsAsync(query);
            var model = new IndexViewModel();
            model.Audits = result;
            model.GetAudits = query;
            return View(model);
        }

        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    var getAudits = new GetAudits() { Page = 1, PageSize = 25 };
        //    var result = await SendRequest<GetAudits, AuditsDto>(getAudits);
        //    var model = new IndexViewModel();
        //    model.Audits = result;
        //    model.GetAudits = getAudits;
        //    return View(model);
        //}

        //protected async Task<TResult> SendRequest<TRequest, TResult>(TRequest request)
        //{
        //    var wrapper = new RequestWrapper<TRequest, TResult>(request, User);
        //    return await _mediator.Send(wrapper);
        //}
    }
}