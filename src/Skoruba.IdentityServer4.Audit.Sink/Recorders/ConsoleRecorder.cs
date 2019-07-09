using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Audit.Sink.Recorders
{
    public class ConsoleRecorder : IRecordAuditActions
    {
        public Task RecordFailure(IAuditArgs auditEventArguments)
        {
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(auditEventArguments));
            return Task.FromResult(0);
        }

        public Task RecordSuccess(IAuditArgs auditEventArguments)
        {
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(auditEventArguments));
            return Task.FromResult(0);
        }
    }
}