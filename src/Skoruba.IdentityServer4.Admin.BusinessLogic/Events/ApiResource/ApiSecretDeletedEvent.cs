using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.ApiResource
{
    public class ApiSecretDeletedEvent : AuditEvent
    {
        public ApiSecretsDto ApiSecret { get; set; }

        public ApiSecretDeletedEvent(ApiSecretsDto apiSecret)
        {
            ApiSecret = apiSecret;
        }
    }
}