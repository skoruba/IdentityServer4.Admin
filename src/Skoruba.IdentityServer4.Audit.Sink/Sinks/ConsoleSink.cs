using Skoruba.IdentityServer4.Audit.Sink.Recorders;
using Microsoft.AspNetCore.Http;

namespace Skoruba.IdentityServer4.Audit.Sink.Sinks
{
    public class ConsoleSink : AuditSinkBase
    {
        public ConsoleSink(IConsoleRecorder auditRecorder, IHttpContextAccessor httpContext) : base(auditRecorder, httpContext)
        {
        }
    }
}