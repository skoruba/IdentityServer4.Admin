using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class RoleClaimsRequestedEvent<TRoleClaimsDto> : AuditEvent
    {
        public TRoleClaimsDto RoleClaims { get; set; }

        public RoleClaimsRequestedEvent(TRoleClaimsDto roleClaims)
        {
            RoleClaims = roleClaims;
        }
    }
}