namespace Skoruba.IdentityServer4.Audit.Sink
{
    public interface IAuditArgs
    {
        /// <summary>
        /// The name of the event
        /// </summary>
        string EventName { get; }

        /// <summary>
        /// The id of the event
        /// </summary>
        string EventId { get; }

        /// <summary>
        /// Defines from where the request originated, such as the client application
        /// </summary>
        AuditArgResource Source { get; }

        /// <summary>
        /// Defines the object which originated the request, such as the user
        /// </summary>
        AuditArgResource Actor { get; }

        /// <summary>
        /// Defines the object that is impacted by the request, such as a user
        /// </summary>
        AuditArgResource Subject { get; }

        /// <summary>
        /// Defines additional details from the event
        /// </summary>
        EventDetail EventDetail { get; }

        /// <summary>
        /// The changes that occurred (if applicable)
        /// </summary>
        string Changes { get; }
    }
}