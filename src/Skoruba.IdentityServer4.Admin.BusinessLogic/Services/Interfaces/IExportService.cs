using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
    public interface IExportService
    {
        Task<byte []> GetExportBytesAsync();
        Task ImportAsync(string txt);
    }
}