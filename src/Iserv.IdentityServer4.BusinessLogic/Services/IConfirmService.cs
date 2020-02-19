using System.Threading.Tasks;

namespace Iserv.IdentityServer4.BusinessLogic.Services
{
    /// <summary>
    /// Сервис подтверждения действий
    /// </summary>    
    public interface IConfirmService : IConfirmByEmailService, IConfirmBySmsService
    {
        
    }
}