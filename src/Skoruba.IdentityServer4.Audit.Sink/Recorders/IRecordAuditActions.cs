using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Audit.Sink.Recorders
{
    public interface IRecordAuditActions
    {
        Task RecordFailure(IAuditArgs auditEventArguments);

        Task RecordSuccess(IAuditArgs auditEventArguments);
    }
}