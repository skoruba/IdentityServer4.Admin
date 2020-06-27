using System.Collections.Generic;

namespace IdentityServer4.Admin.MultiTenancy.Infrastructure
{
    public class TenantResolveResult
    {
        public string TenantIdOrName { get; set; }
        public List<string> AppliedResolvers { get; }
        public TenantResolveResult()
        {
            AppliedResolvers = new List<string>();
        }
    }
}
