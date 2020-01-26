using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using SkorubaIdentityServer4Admin.Admin.Helpers;

namespace SkorubaIdentityServer4Admin.Admin.ExceptionHandling
{
    public class ControllerExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public ControllerExceptionFilterAttribute(ITempDataDictionaryFactory tempDataDictionaryFactory,
            IModelMetadataProvider modelMetadataProvider)
        {
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _modelMetadataProvider = modelMetadataProvider;
        }

        public override void OnException(ExceptionContext context)
        {
            if (!(context.Exception is UserFriendlyErrorPageException) &&
                !(context.Exception is UserFriendlyViewException)) return;

            //Create toastr notification
            if (CreateNotification(context, out var tempData)) return;

            HandleUserFriendlyViewException(context);
            ProcessException(context, tempData);

            //Clear toastr notification from temp
            ClearNotification(tempData);
        }

        private void ClearNotification(ITempDataDictionary tempData)
        {
            tempData.Remove(NotificationHelpers.NotificationKey);
        }

        private bool CreateNotification(ExceptionContext context, out ITempDataDictionary tempData)
        {
            tempData = _tempDataDictionaryFactory.GetTempData(context.HttpContext);
            CreateNotification(NotificationHelpers.AlertType.Error, tempData, context.Exception.Message);

            return !tempData.ContainsKey(NotificationHelpers.NotificationKey);
        }

        private void ProcessException(ExceptionContext context, ITempDataDictionary tempData)
        {
            if (!(context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)) return;

            const string errorViewName = "Error";

            var result = new ViewResult
            {
                ViewName = context.Exception is UserFriendlyViewException
                    ? controllerActionDescriptor.ActionName
                    : errorViewName,
                TempData = tempData,
                ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState)
                {
                    {"Notifications", tempData[NotificationHelpers.NotificationKey]},
                }
            };

            //For UserFriendlyException is necessary return model with latest form state
            if (context.Exception is UserFriendlyViewException exception)
            {
                result.ViewData.Model = exception.Model;
            }

            context.ExceptionHandled = true;
            context.Result = result;
        }

        private void HandleUserFriendlyViewException(ExceptionContext context)
        {
            if (!(context.Exception is UserFriendlyViewException userFriendlyViewException)) return;

            if (userFriendlyViewException.ErrorMessages != null && userFriendlyViewException.ErrorMessages.Any())
            {
                foreach (var message in userFriendlyViewException.ErrorMessages)
                {
                    context.ModelState.AddModelError(message.ErrorKey, message.ErrorMessage);
                }
            }
            else
            {
                context.ModelState.AddModelError(userFriendlyViewException.ErrorKey, context.Exception.Message);
            }
        }
        
        protected void CreateNotification(NotificationHelpers.AlertType type, ITempDataDictionary tempData, string message, string title = "")
        {
            var toast = new NotificationHelpers.Alert
            {
                Type = type,
                Message = message,
                Title = title
            };

            var alerts = new List<NotificationHelpers.Alert>();

            if (tempData.ContainsKey(NotificationHelpers.NotificationKey))
            {
                alerts = JsonConvert.DeserializeObject<List<NotificationHelpers.Alert>>(tempData[NotificationHelpers.NotificationKey].ToString());
                tempData.Remove(NotificationHelpers.NotificationKey);
            }

            alerts.Add(toast);

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var alertJson = JsonConvert.SerializeObject(alerts, settings);

            tempData.Add(NotificationHelpers.NotificationKey, alertJson);
        }
    }
}





