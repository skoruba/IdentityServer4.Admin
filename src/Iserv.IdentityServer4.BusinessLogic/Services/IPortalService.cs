using Iserv.IdentityServer4.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Iserv.IdentityServer4.BusinessLogic.Services
{
    public interface IPortalService
    {
        /// <summary>
        /// Обновление сессии портала
        /// </summary>
        Task UpdateSessionAsync();

        /// <summary>
        /// Получение сессии портала
        /// </summary>
        /// <returns>Сессия портала</returns>
        string GetCookie();

        /// <summary>
        /// Получение Id пользователя через аутентификацию на портале
        /// </summary>
        /// <param name="userName">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>Id пользователя</returns>
        Task<PortalResult<Guid>> GetUserIdByAuthAsync(string userName, string password);

        /// <summary>
        /// Получение данных пользователя с портала
        /// </summary>
        /// <param name="idext">Внешний Id пользователя на портале</param>
        /// <returns>Данные пользователя</returns>
        Task<PortalResult<Dictionary<string, object>>> GetUserAsync(Guid idext);

        /// <summary>
        /// Регистрация нового пользователя на портале
        /// </summary>
        /// <param name="userProfile">Данные нового пользователя</param>
        /// <returns>Id зарегистрированного пользователя</returns>
        Task<PortalResult<Guid>> RegistrateAsync(PortalRegistrationData userProfile);

        /// <summary>
        /// Изменение данных пользовтеля на портале
        /// </summary>
        /// <param name="idext">Внешний id пользователя портала</param>
        /// <param name="values">Новые значения пользователя</param>
        /// <param name="files"></param>
        /// <returns></returns>
        Task<PortalResult> UpdateUserAsync(Guid idext, Dictionary<string, object> values, IEnumerable<FileModel> files = null);
    }
}