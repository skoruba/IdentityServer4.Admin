using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.Client
{
    public class ClientDeletedEvent : AuditEvent
    {
        public ClientDto Client { get; set; }

        public ClientDeletedEvent(ClientDto client)
        {
            Client = client;
        }
    }
}