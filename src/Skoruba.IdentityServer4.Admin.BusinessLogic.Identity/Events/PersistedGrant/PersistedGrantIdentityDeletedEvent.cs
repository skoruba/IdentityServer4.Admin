using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.PersistedGrant
{
    public class PersistedGrantIdentityDeletedEvent : AuditEvent
    {
        public string Key { get; set; }

        public PersistedGrantIdentityDeletedEvent(string key)
        {
            Key = key;
        }
    }
}