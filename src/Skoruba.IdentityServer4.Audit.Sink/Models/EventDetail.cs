using IdentityServer4.Events;

namespace Skoruba.IdentityServer4.Audit.Sink
{
    public class EventDetail : Event
    {
        public string Description { get; set; }

        public EventDetail(Event evt) : base(evt.Category, evt.Name, evt.EventType, evt.Id, evt.Message)
        {
            this.ActivityId = evt.ActivityId;
            this.TimeStamp = evt.TimeStamp;
            this.ProcessId = evt.ProcessId;
            this.LocalIpAddress = evt.LocalIpAddress;
            this.RemoteIpAddress = evt.RemoteIpAddress;
            this.Description = evt.ToString().SafeForFormatted();
        }
    }
}