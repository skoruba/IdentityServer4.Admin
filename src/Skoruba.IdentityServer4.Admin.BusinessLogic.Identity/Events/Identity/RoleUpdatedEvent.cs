using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class RoleUpdatedEvent<TRoleDto> : AuditEvent
    {
        public TRoleDto OriginalRole { get; set; }
        public TRoleDto Role { get; set; }

        public RoleUpdatedEvent(TRoleDto originalRole, TRoleDto role)
        {
            OriginalRole = originalRole;
            Role = role;
        }
    }
}