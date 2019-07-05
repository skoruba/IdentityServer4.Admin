using Skoruba.IdentityServer4.Audit.Sink.Constants;
using IdentityServer4.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Skoruba.IdentityServer4.Audit.Sink.Events
{
    public class ProfileChangedEvent : Event
    {
        protected ProfileChangedEvent()
            : base(AuditEventCategories.UserProfile,
                  "User Password Changed",
                  EventTypes.Success,
                  AuditEventIds.ChangePasswordSuccess)
        {
        }

        public ProfileChangedEvent(string subjectId, string subjectName, string changes)
            : this()
        {
            SubjectId = subjectId;
            SubjectName = subjectName;
            Changes = changes;
        }

        public string SubjectId { get; }
        public string SubjectName { get; }
        public string Changes { get; }
    }
}