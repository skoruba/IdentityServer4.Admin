using Iserv.IdentityServer4.BusinessLogic.Models;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Iserv.IdentityServer4.BusinessLogic.Services
{
    /// <summary>
    /// Сервис управления с акаунтом пользователя
    /// </summary>
    public interface IAccountService<TUser, TKey>
        where TUser : UserIdentity<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Поиск пользователя по телефону
        /// </summary>
        /// <param name="phone">Номер телефона</param>
        /// <returns>Пользователь</returns>
        Task<TUser> FindByPhoneAsync(string phone);

        /// <summary>
        /// Поиск пользователя по внешнему id портала
        /// </summary>
        /// <param name="idext">Внешний id портала</param>
        /// <returns>Пользователь</returns>
        Task<TUser> FindByIdextAsync(Guid idext);

        /// <summary>
        /// Создание нового пользователя из портала
        /// </summary>
        /// <param name="idext">Внешний id пользователя портала</param>
        /// <param name="password">Пароль нового пользователя</param>
        Task CreateUserAsync(Guid idext, string password);

        /// <summary>
        /// Запрос на проверку подленности номера телефона пользователю по смс
        /// </summary>
        /// <param name="phone">Проверяемый номер телефона</param>
        /// <returns>Результат запуска проверки подленности номера телефона пользователю по смс</returns>
        Task RequestCheckPhoneAsync(string phone);

        /// <summary>
        /// Проверка кода верификации номера телефона по смс
        /// </summary>
        /// <param name="phone">Проверяемый номер телефона</param>
        /// <param name="code">Код смс проверяемого номера телефона</param>
        /// <returns>Результат проверки кода</returns>
        void ValidSMSCodeCheckPhone(string phone, string code);

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="model">Данные нового пользователя</param>
        Task<Guid> RegisterAsync(RegistrUserModel model);

        /// <summary>
        /// Обновление данных пользователя
        /// </summary>
        /// <param name="user">Изменяемый пользователь</param>
        /// <param name="values">Данные пользователя</param>
        /// <param name="files">Файлы пользователя</param>
        Task UpdateUserAsync(TUser user, Dictionary<string, object> values, IEnumerable<FileModel> files);

        /// <summary>
        /// Изменение email пользователя
        /// </summary>
        /// <param name="user">Изменяемый пользователь</param>
        /// <param name="email">email пользователя</param>
        Task ChangeEmailAsync(TUser user, string email);

        /// <summary>
        /// Изменение номера телефона пользователя
        /// </summary>
        /// <param name="user">Изменяемый пользователь</param>
        /// <param name="phoneNumber">Номер телефона пользователя</param>
        Task ChangePhoneAsync(TUser user, string phoneNumber);

        /// <summary>
        /// Восстановление пароля по email
        /// </summary>
        /// <param name="email">Email востанвовления</param>
        Task RestorePasswordByEmailAsync(string email);

        /// <summary>
        /// Запрос на восстановление пароля по смс
        /// </summary>
        /// <param name="phone">Номер телефона для востанвовления</param>
        Task RepairPasswordBySMSAsync(string phone);

        /// <summary>
        /// Проверка кода восстановления пароля по смс
        /// </summary>
        /// <param name="phone">Номер телефона для восстанвовления</param>
        /// <param name="code">Код смс восстановления пароля</param>
        void ValidSMSCodeChangePassword(string phone, string code);

        /// <summary>
        /// Изменение пароля по смс
        /// </summary>
        /// <param name="phone">Номер телефона для восстанвовления</param>
        /// <param name="code">Код смс восстановления пароля</param>
        /// <param name="password">Новый пароль</param>
        Task ChangePasswordBySMSAsync(string phone, string code, string password);
    }
}