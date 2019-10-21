using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.Client
{
    public class ClientSecretAddedEvent : AuditEvent
    {
        public ClientSecretsDto ClientSecret { get; set; }

        public ClientSecretAddedEvent(ClientSecretsDto clientSecret)
        {
            ClientSecret = clientSecret;
        }
    }
}