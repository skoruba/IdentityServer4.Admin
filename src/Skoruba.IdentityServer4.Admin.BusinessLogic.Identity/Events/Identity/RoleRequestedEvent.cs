using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class RoleRequestedEvent<TRoleDto> : AuditEvent
    {
        public TRoleDto Role { get; set; }

        public RoleRequestedEvent(TRoleDto role)
        {
            Role = role;
        }
    }
}