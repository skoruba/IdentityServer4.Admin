using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces
{
    public interface ITenantRepository
    {
        Task<List<Tenant>> GetListAsync();
        Task<PagedList<Tenant>> GetListAsync(string search, int page = 1, int pageSize = 10);
        Task<Tenant> FindByNameAsync(string name);
        Task<Tenant> FindByIdAsync(Guid id);
        Task<int> GetCountAsync();
        Task DeleteAsync(Tenant entity);
        Task<Tenant> AddAsync(Tenant entity);
        Task<Tenant> UpdateAsync(Tenant entity);

        Task DeleteEditionAsync(Edition entity);
        Task<Edition> FindEditionByIdAsync(Guid id);
        Task<Edition> FindEditionByNameAsync(string name);
        Task<Edition> AddEditionAsync(Edition entity);
        Task<Edition> UpdateEditionAsync(Edition entity);
        Task<PagedList<Edition>> GetEditionListAsync(string search, int page = 1, int pageSize = 10);
        Task<List<Edition>> GetEditionListAsync();
    }
}
