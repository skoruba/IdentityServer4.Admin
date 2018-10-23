using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces
{
    public interface IIdentityResourceRepository<TDbContext>
        where TDbContext : DbContext, IAdminConfigurationDbContext
    {
        Task<PagedList<IdentityResource>> GetIdentityResourcesAsync(string search, int page = 1, int pageSize = 10);

        Task<IdentityResource> GetIdentityResourceAsync(int identityResourceId);

        Task<bool> CanInsertIdentityResourceAsync(IdentityResource identityResource);

        Task<int> AddIdentityResourceAsync(IdentityResource identityResource);

        Task<int> UpdateIdentityResourceAsync(IdentityResource identityResource);

        Task<int> DeleteIdentityResourceAsync(IdentityResource identityResource);

        Task<int> SaveAllChangesAsync();
    }
}