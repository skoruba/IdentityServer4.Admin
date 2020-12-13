using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class UserRequestedEvent<TUserDto> : AuditEvent
    {
        public TUserDto UserDto { get; set; }

        public UserRequestedEvent(TUserDto userDto)
        {
            UserDto = userDto;
        }
    }
}