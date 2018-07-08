namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity.Base
{
    public class BaseUserRolesDto<TUserId, TRoleId>
    {
        public TUserId UserId { get; set; }

        public TRoleId RoleId { get; set; }
    }
}