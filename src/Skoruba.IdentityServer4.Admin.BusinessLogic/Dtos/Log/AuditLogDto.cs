using System;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Log
{
    public class AuditLogDto
    {
        /// <summary>
        /// Unique identifier for event
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Event name
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// Source of logging events
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Event category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Subject Identifier - who is responsible for current action
        /// </summary>
        public string SubjectIdentifier { get; set; }

        /// <summary>
        /// Subject Name - who is responsible for current action
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// Subject Type - User/Machine
        /// </summary>
        public string SubjectType { get; set; }

        /// <summary>
        /// Subject - some additional data
        /// </summary>
        public string SubjectAdditionalData { get; set; }

        /// <summary>
        /// Information about request/action
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Data which are serialized into JSON format
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Date and time for creating of the event
        /// </summary>
        public DateTime Created { get; set; }
    }
}
