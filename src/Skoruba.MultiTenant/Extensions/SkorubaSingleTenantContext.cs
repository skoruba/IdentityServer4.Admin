using Skoruba.MultiTenant.Configuration;
using Skoruba.MultiTenant.Abstractions;
using Skoruba.MultiTenant;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public class SkorubaSingleTenantContext : ISkorubaTenantContext
    {
        public SkorubaSingleTenantContext(MultiTenantConfiguration multiTenantConfiguration)
        {
            Tenant = new SingleTenant();
            MultiTenantConfiguration = multiTenantConfiguration;
        }

        public ISkorubaTenant Tenant { get; }
        public bool MultiTenantEnabled =>  false;
        public bool TenantResolved => false;
        public bool TenantResolutionRequired => false;
        public string TenantResolutionStrategy => "None";
        public MultiTenantConfiguration MultiTenantConfiguration { get; }

        public void SetTenantId(IHaveTenantId obj)
        {
            // do nothing
        }
    }

    public class SingleTenant : ISkorubaTenant
    {
        public string Id { get; }
        public string Identifier { get; }
        public string Name { get; }
        public string ConnectionString { get; }
        public IDictionary<string, object> Items { get; } = new Dictionary<string, object>();
    }
}