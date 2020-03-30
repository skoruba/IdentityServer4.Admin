using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.ApiResource
{
    public class ApiResourceAddedEvent : AuditEvent
    {
        public ApiResourceDto ApiResource { get; set; }

        public ApiResourceAddedEvent(ApiResourceDto apiResource)
        {
            ApiResource = apiResource;
        }
    }
}