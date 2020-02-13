using System.Threading.Tasks;

namespace Iserv.IdentityServer4.BusinessLogic.Services
{
    public interface IExportService
    {
        Task<byte []> GetExportBytesConfigAsync();
        Task ImportConfigAsync(string txt);
    }
}