using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Grant;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces
{
    public interface IPersistedGrantAspNetIdentityService
    {
        Task<PersistedGrantsDto> GetPersitedGrantsByUsers(string search, int page = 1, int pageSize = 10);
        Task<PersistedGrantsDto> GetPersitedGrantsByUser(string subjectId, int page = 1, int pageSize = 10);
        Task<PersistedGrantDto> GetPersitedGrantAsync(string key);
        Task<int> DeletePersistedGrantAsync(string key);
        Task<int> DeletePersistedGrantsAsync(string userId);
    }
}