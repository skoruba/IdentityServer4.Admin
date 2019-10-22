using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.Client
{
    public class ClientSecretRequestedEvent : AuditEvent
    {
        public ClientSecretsDto ClientSecrets { get; set; }

        public ClientSecretRequestedEvent(ClientSecretsDto clientSecrets)
        {
            ClientSecrets = clientSecrets;
        }
    }
}