using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class UserPasswordChangedEvent<TUserChangePasswordDto> : AuditEvent
    {
        public TUserChangePasswordDto UserPassword { get; set; }

        public UserPasswordChangedEvent(TUserChangePasswordDto userPassword)
        {
            UserPassword = userPassword;
        }
    }
}