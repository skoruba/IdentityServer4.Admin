using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;

namespace SkorubaIdentityServer4Admin.Admin.Api.ExceptionHandling
{
    public class ControllerExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private const string ErrorKey = "Error";

        public override void OnException(ExceptionContext context)
        {
            if (!(context.Exception is UserFriendlyErrorPageException) &&
                !(context.Exception is UserFriendlyViewException)) return;

            HandleUserFriendlyViewException(context);
            ProcessException(context);
        }

        void SetTraceId(string traceIdentifier, ProblemDetails problemDetails)
        {
            var traceId = Activity.Current?.Id ?? traceIdentifier;
            problemDetails.Extensions["traceId"] = traceId;
        }

        private void ProcessException(ExceptionContext context)
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Title = "One or more model validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Instance = context.HttpContext.Request.Path
            };
            
            SetTraceId(context.HttpContext.TraceIdentifier, problemDetails);

            var exceptionResult =  new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = {
                    "application/problem+json",
                    "application/problem+xml" }
            };

            context.ExceptionHandled = true;
            context.Result = exceptionResult;
        }

        private void HandleUserFriendlyViewException(ExceptionContext context)
        {
            if (context.Exception is UserFriendlyViewException userFriendlyViewException)
            {
                if (userFriendlyViewException.ErrorMessages != null && userFriendlyViewException.ErrorMessages.Any())
                {
                    foreach (var message in userFriendlyViewException.ErrorMessages)
                    {
                        context.ModelState.AddModelError(message.ErrorKey ?? ErrorKey, message.ErrorMessage);
                    }
                }
                else
                {
                    context.ModelState.AddModelError(userFriendlyViewException.ErrorKey ?? ErrorKey, context.Exception.Message);
                }
            }

            if (context.Exception is UserFriendlyErrorPageException userFriendlyErrorPageException)
            {
                context.ModelState.AddModelError(userFriendlyErrorPageException.ErrorKey ?? ErrorKey, context.Exception.Message);
            }
        }
    }
}





