using System;
using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.ApiResource
{
    public class ApiSecretRequestedEvent : AuditEvent
    {
        public int ApiResourceId { get; set; }

        public int ApiSecretId { get; set; }

        public string Type { get; set; }

        public DateTime? Expiration { get; set; }

        public ApiSecretRequestedEvent(int apiResourceId, int apiSecretId, string type, DateTime? expiration)
        {
            ApiResourceId = apiResourceId;
            ApiSecretId = apiSecretId;
            Type = type;
            Expiration = expiration;
        }
    }
}