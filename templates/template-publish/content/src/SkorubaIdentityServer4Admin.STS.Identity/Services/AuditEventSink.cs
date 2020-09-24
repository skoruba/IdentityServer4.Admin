using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace SkorubaIdentityServer4Admin.STS.Identity.Services
{
    public class AuditEventSink : DefaultEventSink
    {
        public AuditEventSink(ILogger<DefaultEventService> logger) : base(logger)
        {
        }

        public override Task PersistAsync(Event evt)
        {
            return base.PersistAsync(evt);
        }
    }
}





