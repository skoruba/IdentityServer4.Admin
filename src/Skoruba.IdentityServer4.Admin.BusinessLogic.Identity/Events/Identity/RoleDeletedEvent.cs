using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class RoleDeletedEvent<TRoleDto> : AuditEvent
    {
        public TRoleDto Role { get; set; }

        public RoleDeletedEvent(TRoleDto role)
        {
            Role = role;
        }
    }
}