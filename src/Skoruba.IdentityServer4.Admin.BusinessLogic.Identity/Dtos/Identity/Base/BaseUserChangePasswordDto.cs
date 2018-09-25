namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Base
{
    public class BaseUserChangePasswordDto<TUserId>
    {
        public TUserId UserId { get; set; }
    }
}