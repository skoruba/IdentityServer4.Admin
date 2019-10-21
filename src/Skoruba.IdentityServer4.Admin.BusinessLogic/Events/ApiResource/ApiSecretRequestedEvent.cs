using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.ApiResource
{
    public class ApiSecretRequestedEvent : AuditEvent
    {
        public ApiSecretsDto ApiSecrets { get; set; }

        public ApiSecretRequestedEvent(ApiSecretsDto apiSecrets)
        {
            ApiSecrets = apiSecrets;
        }
    }
}