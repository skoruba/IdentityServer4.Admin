using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.ApiScope
{
    public class ApiScopePropertyDeletedEvent : AuditEvent
    {
        public ApiScopePropertyDeletedEvent(ApiScopePropertiesDto apiScopeProperty)
        {
            ApiScopeProperty = apiScopeProperty;
        }

        public ApiScopePropertiesDto ApiScopeProperty { get; set; }
    }
}