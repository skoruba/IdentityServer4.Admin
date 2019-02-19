using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Grant;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
    public interface IPersistedGrantService
    {
        Task<PersistedGrantsDto> GetPersistedGrantsByUsers(string search, int page = 1, int pageSize = 10);
        Task<PersistedGrantsDto> GetPersistedGrantsByUser(string subjectId, int page = 1, int pageSize = 10);
        Task<PersistedGrantDto> GetPersistedGrantAsync(string key);
        Task<int> DeletePersistedGrantAsync(string key);
        Task<int> DeletePersistedGrantsAsync(string userId);
    }
}