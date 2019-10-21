using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.ApiResource
{
    public class ApiSecretsRequestedEvent : AuditEvent
    {
        public ApiSecretsDto ApiSecrets { get; set; }

        public ApiSecretsRequestedEvent(ApiSecretsDto apiSecrets)
        {
            ApiSecrets = apiSecrets;
        }
    }
}