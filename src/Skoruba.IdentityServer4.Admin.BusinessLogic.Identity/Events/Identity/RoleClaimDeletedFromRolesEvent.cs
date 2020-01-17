using System.Collections.Generic;
using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class RoleClaimDeletedFromRolesEvent : AuditEvent
    {
        public string RoleClaim { get; set; }
        public IEnumerable<string> RoleIds { get; set; }

        public RoleClaimDeletedFromRolesEvent(string roleClaim, IEnumerable<string> roleIds)
        {
            RoleClaim = roleClaim;
            RoleIds = roleIds;
        }
        
    }
}