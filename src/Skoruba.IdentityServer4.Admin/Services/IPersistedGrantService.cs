using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.ViewModels.Grant;

namespace Skoruba.IdentityServer4.Admin.Services
{
    public interface IPersistedGrantService
    {
        Task<PersistedGrantsDto> GetPersitedGrantsByUsers(string search, int page = 1, int pageSize = 10);
        Task<PersistedGrantsDto> GetPersitedGrantsByUser(string subjectId, int page = 1, int pageSize = 10);
        Task<PersistedGrantDto> GetPersitedGrantAsync(string key);
        Task<int> DeletePersistedGrantAsync(string key);
        Task<int> DeletePersistedGrantsAsync(int userId);
    }
}