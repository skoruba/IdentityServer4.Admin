using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.ApiResource
{
    public class ApiResourceScopesRequestedEvent : AuditEvent
    {
        public ApiResourceScopesDto ApiResourceScopes { get; set; }

        public ApiResourceScopesRequestedEvent(ApiResourceScopesDto apiResourceScopes)
        {
            ApiResourceScopes = apiResourceScopes;
        }
    }
}