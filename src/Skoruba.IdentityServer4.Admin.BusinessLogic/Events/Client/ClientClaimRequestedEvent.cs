using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.Client
{
    public class ClientClaimRequestedEvent : AuditEvent
    {
        public ClientClaimsDto ClientClaims { get; set; }

        public ClientClaimRequestedEvent(ClientClaimsDto clientClaims)
        {
            ClientClaims = clientClaims;
        }
    }
}