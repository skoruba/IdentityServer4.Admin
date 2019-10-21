using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.ApiResource
{
    public class ApiResourceDeleted : AuditEvent
    {
        public ApiResourceDto ApiResource { get; }

        public ApiResourceDeleted(ApiResourceDto apiResource)
        {
            ApiResource = apiResource;
        }
    }
}