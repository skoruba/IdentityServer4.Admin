using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.ViewModels.Log;

namespace Skoruba.IdentityServer4.Admin.Services
{
    public interface ILogService
    {
        Task<LogsDto> GetLogsAsync(string search, int page = 1, int pageSize = 10);
    }
}