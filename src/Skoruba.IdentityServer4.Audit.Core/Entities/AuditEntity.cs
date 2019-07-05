using Skoruba.IdentityServer4.Audit.Sink;
using System;

namespace Skoruba.IdentityServer4.Audit.Core.Entities
{
    public class AuditEntity
    {
        public static AuditEntity Create(IAuditArgs audit)
        {
            return new AuditEntity
                (
                    message: null,
                    messageTemplate: null,
                    level: null,
                    timeStamp: audit.EventDetail.TimeStamp,
                    category: audit.EventDetail.Category,
                    action: audit.EventDetail.Name,
                    eventType: audit.EventDetail.EventType.ToString(),
                    sourceType: audit.Source.Type,
                    sourceId: audit.Source.Id,
                    source: audit.Source.Name,
                    actorType: audit.Actor.Type,
                    actorId: audit.Actor.Id,
                    actor: audit.Actor.Name,
                    subjectType: audit.Subject.Type,
                    subjectId: audit.Subject.Id,
                    subject: audit.Subject.Name,
                    remoteIpAddress: audit.EventDetail.RemoteIpAddress,
                    localIpAddress: audit.EventDetail.LocalIpAddress,
                    changes: audit.Changes
                );
        }

        public AuditEntity()
        {
        }

        public AuditEntity(string message, string messageTemplate, string level, DateTime timeStamp, string category, string action, string eventType, string sourceType, string sourceId, string source, string actorType, string actorId, string actor, string subjectType, string subjectId, string subject, string remoteIpAddress, string localIpAddress, string changes)
        {
            Message = message;
            MessageTemplate = messageTemplate;
            Level = level;
            TimeStamp = timeStamp;
            Category = category;
            Action = action;
            EventType = eventType;
            SourceType = sourceType;
            SourceId = sourceId;
            Source = source;
            ActorType = actorType;
            ActorId = actorId;
            Actor = actor;
            SubjectType = subjectType;
            SubjectId = subjectId;
            Subject = subject;
            RemoteIpAddress = remoteIpAddress;
            LocalIpAddress = localIpAddress;
            Changes = changes;
        }

        public long Id { get; set; }
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The category of the action
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The action performed, commonly the event name
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// The loglevel or type of event, e.g. Success, Failure
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// The originating source type
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// The originating source id
        /// </summary>
        public string SourceId { get; set; }

        /// <summary>
        /// The originating name
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The type of actor, e.g., User, WebClient
        /// </summary>
        public string ActorType { get; set; }

        /// <summary>
        /// The id of the actor
        /// </summary>
        public string ActorId { get; set; }

        /// <summary>
        /// The name of the actor, e.g., Username or Displayname
        /// </summary>
        public string Actor { get; set; }

        /// <summary>
        /// The type of the subject modified (if applicable)
        /// </summary>
        public string SubjectType { get; set; }

        /// <summary>
        /// The id of the subject modified (if applicable)
        /// </summary>
        public string SubjectId { get; set; }

        /// <summary>
        /// The name of the subject modified (if applicable)
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// THe remote IP Address
        /// </summary>
        public string RemoteIpAddress { get; set; }

        /// <summary>
        /// The local IP Address
        /// </summary>
        public string LocalIpAddress { get; set; }

        /// <summary>
        /// The changes that occurred (if applicable)
        /// </summary>
        public string Changes { get; set; }
    }
}