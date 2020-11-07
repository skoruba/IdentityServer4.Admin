using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces
{
    public interface IApiScopeRepository
    {
        Task<PagedList<ApiScope>> GetApiScopesAsync(int page = 1, int pageSize = 10);

        Task<ApiScope> GetApiScopeAsync(int apiScopeId);

        Task<int> AddApiScopeAsync(ApiScope apiScope);

        Task<int> UpdateApiScopeAsync(ApiScope apiScope);

        Task<int> DeleteApiScopeAsync(ApiScope apiScope);

        Task<bool> CanInsertApiScopeAsync(ApiScope apiScope);

        Task<ICollection<string>> GetApiScopesAsync(string scope, int limit = 0);

        Task<int> SaveAllChangesAsync();

        bool AutoSaveChanges { get; set; }
    }
}