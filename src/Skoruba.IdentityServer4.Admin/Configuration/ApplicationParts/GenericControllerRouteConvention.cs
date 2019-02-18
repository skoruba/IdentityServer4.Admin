using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Skoruba.IdentityServer4.Admin.Configuration.ApplicationParts
{
    public class GenericControllerRouteConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsGenericType)
            {
                controller.ControllerName = controller.ControllerType.Name.Substring(0, controller.ControllerType.Name.IndexOf('`') - 10);
            }
        }
    }
}