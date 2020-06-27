using System;
using System.Threading.Tasks;

namespace IdentityServer4.Admin.MultiTenancy.Infrastructure
{
    public interface ITenantStore
    {
        Task<TenantConfiguration> FindAsync(string name);

        Task<TenantConfiguration> FindAsync(Guid id);
    }
}
