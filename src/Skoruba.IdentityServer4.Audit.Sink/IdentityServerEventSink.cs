using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Audit.Sink.Sinks;

namespace Skoruba.IdentityServer4.Audit.Sink
{
    public class IdentityServerEventSink : DefaultEventSink, IAuditSink
    {
        public IdentityServerEventSink(ILogger<DefaultEventService> logger) : base(logger)
        {
        }
    }
}