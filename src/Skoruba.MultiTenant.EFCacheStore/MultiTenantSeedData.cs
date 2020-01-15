using System.Collections.Generic;

namespace Skoruba.MultiTenant.EFCacheStore
{
    public class MultiTenantSeedData
    {
        public List<Tenant> Tenants { get; set; }
    }

    public class Tenant
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public Dictionary<string, object> Items { get; set; }
    }
}