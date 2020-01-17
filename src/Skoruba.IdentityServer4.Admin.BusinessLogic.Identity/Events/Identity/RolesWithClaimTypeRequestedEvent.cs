using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class RolesWithClaimTypeRequestedEvent : AuditEvent
    {
        public string ClaimType { get; set; }

        public RolesWithClaimTypeRequestedEvent(string claimType)
        {
            ClaimType = claimType;
        }
    }
}