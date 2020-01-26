using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces
{
    public interface IExportIdentityService
    {
        Task ImportUsersAsync(string txt);
    }
}