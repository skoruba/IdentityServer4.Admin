using Microsoft.AspNetCore.Http;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using IdentityServer4.Events;
using Skoruba.IdentityServer4.Audit.Core.Events;
using System.Diagnostics;
using IdentityServer4.Configuration;

namespace Skoruba.IdentityServer4.Audit.Core
{
    public class AuditEventService
    {
        /// <summary>
        /// The context
        /// </summary>
        protected readonly IHttpContextAccessor Context;

        /// <summary>
        /// The clock
        /// </summary>
        protected readonly ISystemClock Clock;

        /// <summary>
        /// The IdentityServer4 event service
        /// </summary>
        protected readonly IEventService EventService;

        public AuditEventService(IEventService eventService, IHttpContextAccessor context, ISystemClock clock)
        {
            EventService = eventService;
            Context = context;
            Clock = clock;
        }

        public async Task RaiseAsync(Event evt)
        {
            // update event details
            if (evt is IAuditEvent)
            {
                evt.ActivityId = Context.HttpContext.TraceIdentifier;
                evt.TimeStamp = Clock.UtcNow.UtcDateTime;
                evt.ProcessId = Process.GetCurrentProcess().Id;
                if (Context.HttpContext.Connection.LocalIpAddress != null)
                {
                    evt.LocalIpAddress = Context.HttpContext.Connection.LocalIpAddress.ToString() + ":" + Context.HttpContext.Connection.LocalPort;
                }
                else
                {
                    evt.LocalIpAddress = "unknown";
                }

                if (Context.HttpContext.Connection.RemoteIpAddress != null)
                {
                    evt.RemoteIpAddress = Context.HttpContext.Connection.RemoteIpAddress.ToString();
                }
                else
                {
                    evt.RemoteIpAddress = "unknown";
                }
            }

            // pass event on through the IdentityServer4 event service
            await EventService.RaiseAsync(evt);
        }
    }
}