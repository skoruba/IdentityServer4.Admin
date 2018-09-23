namespace Skoruba.IdentityServer4.Admin.AspNetIdentity.Dtos.Identity.Base
{
    public class BaseUserChangePasswordDto<TUserId>
    {
        public TUserId UserId { get; set; }
    }
}