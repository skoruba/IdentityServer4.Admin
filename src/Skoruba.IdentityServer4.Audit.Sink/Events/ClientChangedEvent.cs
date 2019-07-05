using IdentityServer4.Events;
using IdentityServer4.Models;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.Audit.Sink.Constants;

namespace Skoruba.IdentityServer4.Audit.Sink.Events
{
    public class ClientChangedEvent : Event
    {
        public ClientChangedEvent(string sourceId, string sourceName, string clientId, string clientName, string changes)
            : base(AuditEventCategories.Client, "Client Changed", EventTypes.Success, AuditEventIds.ClientChangedSuccess, "Client values were successfully changed.")
        {
            SourceId = sourceId;
            SourceName = sourceName;
            Subjectid = clientId;
            SubjectName = clientName;
            Changes = changes;
        }

        public string Subjectid { get; }
        public string SubjectName { get; }
        public string SourceId { get; }
        public string SourceName { get; }
        public string Changes { get; }
    }
}