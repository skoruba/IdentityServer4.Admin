using System.Threading.Tasks;

namespace Iserv.IdentityServer4.BusinessLogic.Services
{
    public interface IExportIdentityService
    {
        Task ImportUsersAsync(string txt);
    }
}