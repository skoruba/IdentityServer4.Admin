using Skoruba.IdentityServer4.Audit.Sink.Recorders;
using IdentityServer4.Events;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Skoruba.IdentityServer4.Audit.Sink.Sinks
{
    public abstract class AuditSinkBase : IAuditSink
    {
        private readonly IRecordAuditActions _auditRecorder;
        private readonly IHttpContextAccessor _httpContext;

        internal IAdapterFactory Factory { get; set; } = new AdapterFactory();

        public AuditSinkBase(IRecordAuditActions auditRecorder, IHttpContextAccessor httpContext)
        {
            _auditRecorder = auditRecorder ?? throw new ArgumentNullException();
            _httpContext = httpContext;
        }

        public Task PersistAsync(Event evt)
        {
            var auditArgument = Factory.Create(evt, _httpContext);

            if (auditArgument != null)
            {
                if (evt.EventType == EventTypes.Success || evt.EventType == EventTypes.Information)
                {
                    return _auditRecorder.RecordSuccess(auditArgument);
                }

                return _auditRecorder.RecordFailure(auditArgument);
            }

            return Task.CompletedTask;
        }
    }
}