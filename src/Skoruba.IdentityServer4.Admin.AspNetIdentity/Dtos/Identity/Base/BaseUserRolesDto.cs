namespace Skoruba.IdentityServer4.Admin.AspNetIdentity.Dtos.Identity.Base
{
    public class BaseUserRolesDto<TUserDtoKey, TRoleDtoKey>
    {
        public TUserDtoKey UserId { get; set; }

        public TRoleDtoKey RoleId { get; set; }
    }
}