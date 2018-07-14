namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity.Base
{
    public class BaseUserChangePasswordDto<TUserId>
    {
        public TUserId UserId { get; set; }
    }
}