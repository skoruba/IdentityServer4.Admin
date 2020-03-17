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
        Task<PortalResult> UpdateSessionAsync();

        /// <summary>
        /// Получение сессии портала
        /// </summary>
        /// <returns>Сессия портала</returns>
        string GetCookie();

        /// <summary>
        /// Получение Id пользователя через аутентификацию на портале
        /// </summary>
        /// <param name="loginTypes">Тип логина</param>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>Внешний Id пользователя портала</returns>
        Task<PortalResult<Guid>> GetUserIdByAuthAsync(ELoginTypes loginTypes, string login, string password);

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
        /// <returns>Внешний Id зарегистрированного пользователя портала</returns>
        Task<PortalResult<Guid>> RegisterAsync(PortalRegistrationData userProfile);

        /// <summary>
        /// Изменение данных пользовтеля на портале
        /// </summary>
        /// <param name="idext">Внешний id пользователя портала</param>
        /// <param name="values">Новые значения пользователя</param>
        /// <param name="files">Файлы пользователя</param>
        Task<PortalResult> UpdateUserAsync(Guid idext, Dictionary<string, object> values, IEnumerable<FileModel> files = null);

        /// <summary>
        /// Изменение пароля пользователя
        /// </summary>
        /// <param name="idext">Внешний id пользователя портала</param>
        /// <param name="password">Новый пароль пользователя</param>
        Task<PortalResult> UpdatePasswordAsync(Guid idext, string password);
        
        /// <summary>
        /// Восстановление пароля пользователя через email
        /// </summary>
        /// <param name="email">Email пользователя портала</param>
        Task<PortalResult> RestorePasswordByEmailAsync(string email);
    }
}