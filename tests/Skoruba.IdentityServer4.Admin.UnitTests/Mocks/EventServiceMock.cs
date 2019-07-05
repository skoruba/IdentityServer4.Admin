using FluentAssertions;
using IdentityServer4.Events;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mocks
{
    public class EventServiceMock : IEventService
    {
        private Dictionary<Type, object> _events = new Dictionary<Type, object>();

        public Task RaiseAsync(Event evt)
        {
            _events.Add(evt.GetType(), evt);
            return Task.CompletedTask;
        }

        public T AssertEventWasRaised<T>()
            where T : class
        {
            _events.ContainsKey(typeof(T)).Should().BeTrue();
            return (T)_events.Where(x => x.Key == typeof(T)).Select(x => x.Value).First();
        }

        public bool CanRaiseEventType(EventTypes evtType)
        {
            return true;
        }
    }
}