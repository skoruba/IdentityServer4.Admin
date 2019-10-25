using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class UserRoleDeletedEvent<TUserRolesDto> : AuditEvent
    {
        public TUserRolesDto Role { get; set; }

        public UserRoleDeletedEvent(TUserRolesDto role)
        {
            Role = role;
        }
    }
}