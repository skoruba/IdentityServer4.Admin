using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.Data.Entities;
using Skoruba.IdentityServer4.Admin.ViewModels.Common;

namespace Skoruba.IdentityServer4.Admin.Data.Repositories
{
    public interface ILogRepository
    {
        Task<PagedList<Log>> GetLogsAsync(string search, int page = 1, int pageSize = 10);
    }
}