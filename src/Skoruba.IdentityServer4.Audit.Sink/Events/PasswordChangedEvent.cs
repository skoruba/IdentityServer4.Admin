using Skoruba.IdentityServer4.Audit.Sink.Constants;
using IdentityServer4.Events;
using System.Security.Principal;
using IdentityServer4.Extensions;

namespace Skoruba.IdentityServer4.Audit.Sink.Events
{
    public class PasswordChangedEvent : Event
    {
        protected PasswordChangedEvent()
            : base(AuditEventCategories.UserProfile,
                  "User Password Changed",
                  EventTypes.Success,
                  AuditEventIds.ChangePasswordSuccess)
        {
        }

        public PasswordChangedEvent(string subjectId, string subjectName)
            : this()
        {
            SubjectId = subjectId;
            SubjectName = subjectName;
        }

        public string SubjectId { get; }
        public string SubjectName { get; }
    }
}