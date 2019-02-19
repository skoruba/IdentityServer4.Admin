using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Log;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
    public interface ILogService
    {
        Task<LogsDto> GetLogsAsync(string search, int page = 1, int pageSize = 10);
    }
}