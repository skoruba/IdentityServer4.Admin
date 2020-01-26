using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class UserClaimsRequestedEvent<TUserClaimsDto> : AuditEvent
    {
        public TUserClaimsDto Claims { get; set; }

        public UserClaimsRequestedEvent(TUserClaimsDto claims)
        {
            Claims = claims;
        }
    }
}