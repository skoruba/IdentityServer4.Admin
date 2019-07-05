using Skoruba.IdentityServer4.Audit.Sink;
using Skoruba.IdentityServer4.Audit.Sink.Recorders;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Audit.EntityFramework.Recorders
{
    public class EntityFrameworkRecorder : IRecordAuditActions
    {
        private readonly AuditDbContext _dbContext;

        public EntityFrameworkRecorder(AuditDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task RecordFailure(IAuditArgs auditEventArguments)
        {
            return Write(auditEventArguments, false);
        }

        public Task RecordSuccess(IAuditArgs auditEventArguments)
        {
            return Write(auditEventArguments, true);
        }

        private Task Write(IAuditArgs args, bool success)
        {
            return Task.CompletedTask;
        }
    }
}