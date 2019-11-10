using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class UserClaimsDeletedEvent<TUserClaimsDto> : AuditEvent
    {
        public TUserClaimsDto Claim { get; set; }

        public UserClaimsDeletedEvent(TUserClaimsDto claim)
        {
            Claim = claim;
        }
    }
}