using System;

namespace Skoruba.IdentityServer4.Audit.Core.Dtos
{
    public class AuditDto
    {
        public long Id { get; set; }
        public string Level { get; set; }
        public string LogEvent { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string Category { get; set; }
        public string Action { get; set; }
        public string EventType { get; set; }
        public string SourceType { get; set; }
        public string SourceId { get; set; }
        public string Source { get; set; }
        public string ActorType { get; set; }
        public string ActorId { get; set; }
        public string Actor { get; set; }
        public string SubjectType { get; set; }
        public string SubjectId { get; set; }
        public string Subject { get; set; }
        public string RemoteIpAddress { get; set; }
        public string LocalIpAddress { get; set; }
        public string Changes { get; set; }
    }
}