using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.ApiResource
{
    public class ApiResourceUpdatedEvent : AuditEvent
    {
        public ApiResourceDto OriginalApiResource { get; set; }
        public ApiResourceDto ApiResource { get; set; }

        public ApiResourceUpdatedEvent(ApiResourceDto originalApiResource, ApiResourceDto apiResource)
        {
            OriginalApiResource = originalApiResource;
            ApiResource = apiResource;
        }
    }
}