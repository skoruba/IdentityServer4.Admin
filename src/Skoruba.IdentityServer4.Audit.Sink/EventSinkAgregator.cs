using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Audit.Sink.Sinks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Audit.Sink
{
    public class EventSinkAggregator : IEventSink
    {
        private readonly ILogger<EventSinkAggregator> _logger;
        private readonly IEnumerable<IAuditSink> _sinks;

        public EventSinkAggregator(IEnumerable<IAuditSink> sinks, ILogger<EventSinkAggregator> logger)
        {
            _sinks = sinks;
            _logger = logger;
        }

        public Task PersistAsync(Event evt)
        {
            var eventSinkTasks = new List<Task>();

            foreach (var eventSink in _sinks)
            {
                eventSinkTasks.Add(ProtectedExecution(() => eventSink.PersistAsync(evt)));
            }

            return Task.WhenAll(eventSinkTasks);
        }

        private async Task ProtectedExecution(Func<Task> persistAsync)
        {
            try
            {
                await persistAsync();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
#if DEBUG
                throw e;
#endif
            }
        }
    }
}