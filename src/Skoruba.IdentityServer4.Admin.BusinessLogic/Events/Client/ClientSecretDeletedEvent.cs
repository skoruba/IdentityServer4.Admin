using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.Client
{
    public class ClientSecretDeletedEvent : AuditEvent
    {
        public ClientSecretsDto ClientSecret { get; set; }

        public ClientSecretDeletedEvent(ClientSecretsDto clientSecret)
        {
            ClientSecret = clientSecret;
        }
    }
}