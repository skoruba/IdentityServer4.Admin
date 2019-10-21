using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.ApiResource
{
    public class ApiResourceAdded : AuditEvent
    {
        public ApiResourceDto ApiResource { get; }

        public ApiResourceAdded(ApiResourceDto apiResource)
        {
            ApiResource = apiResource;
        }
    }
}