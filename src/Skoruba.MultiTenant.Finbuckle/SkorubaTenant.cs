using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using Skoruba.MultiTenant.Abstractions;
using Skoruba.MultiTenant.Configuration;
using System;
using System.Collections.Generic;

namespace Skoruba.MultiTenant.Finbuckle
{
    public class SkorubaTenant : ISkorubaT
    {
        public SkorubaTenant(TenantInfo tenantInfo)
        {
            Tenant = tenantInfo;
        }
        public string Id => Tenant?.Id;
        public string Name => Tenant?.Name;
        public string Identifier => Tenant?.Identifier;
        public string ConnectionString => Tenant?.ConnectionString;
        public IDictionary<string, object> Items => Tenant?.Items;
        private TenantInfo Tenant { get; }
    }
}