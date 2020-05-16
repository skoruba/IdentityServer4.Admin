using Microsoft.AspNetCore.Http;
using Skoruba.MultiTenant.Abstractions;
using System.Collections.Generic;

namespace Skoruba.MultiTenant.IdentityServer
{
    /// <summary>
    /// Allows the tenant to be unresolved for Identity Server endpoint requests.
    /// </summary>
    public class TenantNotRequiredForIdentityServerEndpoints : IValidateTenantRequirement
    {
        private readonly IEnumerable<IdentityServer4.Hosting.Endpoint> _endpoints;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantNotRequiredForIdentityServerEndpoints(IEnumerable<IdentityServer4.Hosting.Endpoint> endpoints, IHttpContextAccessor httpContextAccessor)
        {
            _endpoints = endpoints;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool TenantIsRequired()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            
            // TODO: Can request and/or path be null?
            var path = httpContext.Request.Path.Value;
            
            foreach(var endpoint in _endpoints)
            {
                // TODO: Are null checks required?
                if (path.ToUpper().EndsWith(endpoint.Path.Value.ToUpper()))
                {
                    return false;
                }
            }

            return true;
        }
    }
}