using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdentityServer4.Admin.MultiTenancy.Infrastructure
{
    [Serializable]
    public class TenantConfiguration
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> ConnectionStrings { get; set; }

        public TenantConfiguration()
        {

        }

        public TenantConfiguration(Guid id, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            Id = id;
            Name = name;
            ConnectionStrings = new Dictionary<string, string>();
        }
    }
}
