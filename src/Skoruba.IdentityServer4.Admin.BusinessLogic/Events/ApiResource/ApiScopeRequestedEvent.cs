﻿using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Events.ApiResource
{
    public class ApiScopeRequestedEvent : AuditEvent
    {
        public ApiScopesDto ApiScope { get; set; }

        public ApiScopeRequestedEvent(ApiScopesDto apiScope)
        {
            ApiScope = apiScope;
        }
    }
}