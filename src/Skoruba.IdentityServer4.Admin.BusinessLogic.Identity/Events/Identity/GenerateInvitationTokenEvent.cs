using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class GenerateInvitationTokenEvent : AuditEvent
    {
        public string UserName { get; set; }

        public GenerateInvitationTokenEvent(string userName)
        {
            UserName = userName;
        }
    }
}