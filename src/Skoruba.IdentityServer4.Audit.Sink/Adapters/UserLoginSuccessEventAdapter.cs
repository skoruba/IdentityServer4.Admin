using IdentityServer4.Events;
using Microsoft.AspNetCore.Http;
using System;

namespace Skoruba.IdentityServer4.Audit.Sink.Adapters
{
    public class UserLoginSuccessEventAdapter : IAuditArgs
    {
        public UserLoginSuccessEvent Event { get; }

        public UserLoginSuccessEventAdapter(UserLoginSuccessEvent evt, IHttpContextAccessor httpContext)
        {
            Event = evt ?? throw new ArgumentNullException(nameof(evt));
        }

        public string EventName => Event.Name;
        public string EventId => Event.Id.ToString();
        public EventDetail EventDetail => new EventDetail(Event);
        public AuditArgResource Source => new AuditArgResource(Event.Endpoint, "IdentityServer", AuditArgResource.AppType);
        public AuditArgResource Actor => new AuditArgResource(Event.SubjectId, Event.DisplayName, AuditArgResource.UserType);
        public AuditArgResource Subject => new AuditArgResource(null, null, null);
        public string Changes { get; }
    }
}