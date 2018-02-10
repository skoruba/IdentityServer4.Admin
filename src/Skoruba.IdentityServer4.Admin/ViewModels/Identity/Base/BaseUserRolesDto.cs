namespace Skoruba.IdentityServer4.Admin.ViewModels.Identity.Base
{
    public class BaseUserRolesDto<TUserId, TRoleId>
    {
        public TUserId UserId { get; set; }

        public TRoleId RoleId { get; set; }
    }
}