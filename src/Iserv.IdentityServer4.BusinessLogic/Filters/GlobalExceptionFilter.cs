using Iserv.IdentityServer4.BusinessLogic.ExceptionHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Iserv.IdentityServer4.BusinessLogic.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var status = context.HttpContext.Response.StatusCode == HttpStatusCode.OK.GetHashCode() ? HttpStatusCode.BadRequest : (HttpStatusCode)context.HttpContext.Response.StatusCode;
            string title = "Identity Server 4";
            if (context.Exception is PortalException) {
                title = "Portal";
            }
            if (!(context.Exception is ValidationException))
            {
                _logger.LogError(context.Exception.Message, context.Exception);
            }
            context.Result = new JsonResult(new
            {
                status,
                title,
                error = context.Exception.Message
            });
        }
    }
}
