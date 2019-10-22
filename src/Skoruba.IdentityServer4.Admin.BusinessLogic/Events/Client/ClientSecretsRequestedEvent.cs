using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.Client
{
    public class ClientSecretsRequestedEvent : AuditEvent
    {
        public ClientSecretsDto ClientSecrets { get; set; }

        public ClientSecretsRequestedEvent(ClientSecretsDto clientSecrets)
        {
            ClientSecrets = clientSecrets;
        }
    }
}