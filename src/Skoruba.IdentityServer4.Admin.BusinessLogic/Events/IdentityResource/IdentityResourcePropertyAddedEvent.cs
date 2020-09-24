using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.IdentityResource
{
    public class IdentityResourcePropertyAddedEvent : AuditEvent
    {
        public IdentityResourcePropertiesDto IdentityResourceProperty { get; set; }

        public IdentityResourcePropertyAddedEvent(IdentityResourcePropertiesDto identityResourceProperty)
        {
            IdentityResourceProperty = identityResourceProperty;
        }
    }
}