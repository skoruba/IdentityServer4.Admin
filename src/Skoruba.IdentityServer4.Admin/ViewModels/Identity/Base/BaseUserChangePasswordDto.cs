namespace Skoruba.IdentityServer4.Admin.ViewModels.Identity.Base
{
    public class BaseUserChangePasswordDto<TUserId>
    {
        public TUserId UserId { get; set; }
    }
}