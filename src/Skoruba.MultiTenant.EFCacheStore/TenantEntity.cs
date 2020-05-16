using System.Collections.Generic;

namespace Skoruba.MultiTenant.Stores
{
    public class TenantEntity : ITenantEntity
    {
        public string ConnectionString { get; set; }
        public string Id { get; set; }
        public string Identifier { get; set; }
        public IDictionary<string, object> Items { get; set; }
        public string Name { get; set; }
    }
}