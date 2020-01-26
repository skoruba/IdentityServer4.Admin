using System;
using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.Log
{
    public class LogsDeletedEvent : AuditEvent
    {
        public DateTime DeleteOlderThan { get; set; }

        public LogsDeletedEvent(DateTime deleteOlderThan)
        {
            DeleteOlderThan = deleteOlderThan;
        }
    }
}