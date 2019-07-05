using IdentityServer4.Events;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;
using System;

namespace Skoruba.IdentityServer4.Audit.Sink.Adapters
{
    public class TokenIssuedFailureEventAdapter : IAuditArgs
    {
        private readonly IHttpContextAccessor _httpContext;

        public TokenIssuedFailureEvent Event { get; }

        public TokenIssuedFailureEventAdapter(TokenIssuedFailureEvent evt, IHttpContextAccessor httpContext)
        {
            Event = evt ?? throw new ArgumentNullException(nameof(evt));
            _httpContext = httpContext;
        }

        public string EventName => Event.Name;
        public string EventId => Event.Id.ToString();
        public EventDetail EventDetail => new EventDetail(Event);
        public AuditArgResource Source => new AuditArgResource(Event.ClientId, Event.ClientName, AuditArgResource.AppType);
        public AuditArgResource Actor => new AuditArgResource(Event.SubjectId, _httpContext.HttpContext.User.GetDisplayName(), AuditArgResource.UserType);
        public AuditArgResource Subject => new AuditArgResource(null, null, null);
        public string Changes { get; }
    }
}