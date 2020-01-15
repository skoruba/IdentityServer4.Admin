using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using Skoruba.MultiTenant.Abstractions;
using Skoruba.MultiTenant.Configuration;
using System;
using System.Collections.Generic;

namespace Skoruba.MultiTenant.Finbuckle
{
    public class SkorubaTenant : ISkorubaTenant
    {

        public SkorubaTenant(IHttpContextAccessor httpContextAccessor, TenantInfo tenantInfo = null, ValidateTenantRequirement validateTenantRequirement = null)
        {
            // TODO: Why is tenantInfo null when the multitenantcontext is not null?
            Tenant = tenantInfo ?? httpContextAccessor.HttpContext.GetMultiTenantContext().TenantInfo;
            TenantResolutionRequired = validateTenantRequirement?.TenantIsRequired() ?? MultiTenantConstants.MultiTenantEnabled;
        }

        public string Id => Tenant?.Id;
        public string Name => Tenant?.Name;
        public string Identifier => Tenant?.Identifier;
        public string ConnectionString => Tenant?.ConnectionString;
        public IDictionary<string, object> Items => Tenant?.Items;
        public bool TenantResolved => Tenant != null;
        private TenantInfo Tenant { get; }
        public bool TenantResolutionRequired { get; }
    }
}