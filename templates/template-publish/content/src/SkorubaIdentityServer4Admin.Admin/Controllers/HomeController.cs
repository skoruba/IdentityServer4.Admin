using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkorubaIdentityServer4Admin.Admin.Configuration.Constants;
using SkorubaIdentityServer4Admin.Admin.ExceptionHandling;

namespace SkorubaIdentityServer4Admin.Admin.Controllers
{
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    public class HomeController : BaseController
    {
        private readonly ILogger<ConfigurationController> _logger;

        public HomeController(ILogger<ConfigurationController> logger) : base(logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Error()
        {
            // Get the details of the exception that occurred
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature == null) return View();

            // Get which route the exception occurred at
            string routeWhereExceptionOccurred = exceptionFeature.Path;

            // Get the exception that occurred
            Exception exceptionThatOccurred = exceptionFeature.Error;

            // Log the exception
            _logger.LogError(exceptionThatOccurred, $"Exception at route {routeWhereExceptionOccurred}");

            return View();
        }
    }
}