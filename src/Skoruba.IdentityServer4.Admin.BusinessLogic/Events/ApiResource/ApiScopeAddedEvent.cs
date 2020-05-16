using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.ApiResource
{
    public class ApiScopeAddedEvent : AuditEvent
    {
        public ApiScopesDto ApiScope { get; set; }

        public ApiScopeAddedEvent(ApiScopesDto apiScope)
        {
            ApiScope = apiScope;
        }
    }
}