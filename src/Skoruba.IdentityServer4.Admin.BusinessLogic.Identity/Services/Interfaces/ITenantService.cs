using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Tenant;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces
{
    public interface ITenantService
    {
        Task<TenantDto> GetTenantAsync(string id);

        Task<TenantsDto> GetTenantsAsync(int page = 1, int pageSize = 10);

        Task<IdentityResult> UpdateTenantAsync();

        Task<TenantDto> AddTenantAsync(TenantDto dto);

        Task<IdentityResult> DisableTenantAsync(string id);
    }
}