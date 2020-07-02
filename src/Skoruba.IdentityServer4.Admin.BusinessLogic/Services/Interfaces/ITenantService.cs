using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration.Tenants;
using System;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
    public interface ITenantService
    {
        Task<TenantDto> GetAsync(string id);
        Task<TenantsDto> GetListAsync(string search, int page = 1, int pageSize = 10);
        Task<TenantDto> CreateAsync(CreateTenantDto input);
        Task<TenantDto> UpdateAsync(UpdateTenantDto input);
        Task DeleteAsync(Guid id);
    }
}
