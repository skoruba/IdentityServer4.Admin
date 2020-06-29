using IdentityServer4.Admin.MultiTenancy.Infrastructure;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Stores
{
    public class TenantStore : ITenantStore
    {
        private ITenantRepository TenantRepository { get; }
        private ICurrentTenant CurrentTenant { get; }

        public TenantStore(
            ITenantRepository tenantRepository,
            ICurrentTenant currentTenant)
        {
            TenantRepository = tenantRepository;
            CurrentTenant = currentTenant;
        }

        public async Task<TenantConfiguration> FindAsync(string name)
        {
            using (CurrentTenant.Change(null))
            {
                var tenant = await TenantRepository.FindByNameAsync(name);
                if (tenant == null)
                {
                    return null;
                }
                return await MapToTenantConfiguration(tenant);
            }
        }

        public async Task<TenantConfiguration> FindAsync(Guid id)
        {
            using (CurrentTenant.Change(null))
            {
                var tenant = await TenantRepository.FindByIdAsync(id);
                if (tenant == null)
                {
                    return null;
                }
                return await MapToTenantConfiguration(tenant);
            }
        }

        private Task<TenantConfiguration> MapToTenantConfiguration(Tenant tenant)
        {
            var response = new TenantConfiguration(tenant.Id, tenant.Name);
            response.ConnectionStrings = tenant.ConnectionStrings.ToDictionary(x => x.Name, x => x.Value);
            return Task.FromResult(response);
        }
    }
}
