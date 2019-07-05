using Microsoft.AspNetCore.Http;
using Skoruba.IdentityServer4.Audit.EntityFramework.Recorders;
using Skoruba.IdentityServer4.Audit.Sink.Recorders;
using Skoruba.IdentityServer4.Audit.Sink.Sinks;

namespace Skoruba.IdentityServer4.Audit.EntityFramework.Sink
{
    public class EntityFrameworkSink : AuditSinkBase
    {
        public EntityFrameworkSink(EntityFrameworkRecorder auditRecorder, IHttpContextAccessor httpContext) : base(auditRecorder, httpContext)
        {
        }
    }
}