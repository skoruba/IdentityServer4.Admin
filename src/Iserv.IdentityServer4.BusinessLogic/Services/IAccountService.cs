using Iserv.IdentityServer4.BusinessLogic.Models;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

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
        /// Поиск пользователя по email
        /// </summary>
        /// <param name="email">Enail телефона</param>
        /// <returns>Пользователь</returns>
        Task<TUser> FindByEmailAsync(string email);
        
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
        /// Получение дополнительных полей пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Дополнительные поля пользователя</returns>
        Task<Dictionary<string, string>> GetExtraFieldsAsync(TUser user);

        /// <summary>
        /// Создание нового пользователя из портала
        /// </summary>
        /// <param name="idext">Внешний id пользователя портала</param>
        /// <param name="password">Пароль нового пользователя</param>
        Task<IdentityResult> CreateUserFromPortalAsync(Guid idext, string password);

        /// <summary>
        /// Обновление данных пользователя из портала
        /// </summary>
        /// <param name="idext">Внешний Id пользователя портала</param>
        Task<IdentityResult> UpdateUserFromPortalAsync(Guid idext);
        
        /// <summary>
        /// Обновление данных пользователя из портала
        /// </summary>
        /// <param name="user">Изменяемый пользователь</param>
        Task<IdentityResult> UpdateUserFromPortalAsync(TUser user);

        /// <summary>
        /// Запрос на проверку подленности email пользователю по email
        /// </summary>
        /// <param name="email">Проверяемый email</param>
        /// <param name="validatingUser">Необходимость проверки пользоватля по email</param>
        /// <returns>Результат запуска проверки подленности email по email</returns>
        Task RequestCheckEmailAsync(string email, bool validatingUser = true);

        /// <summary>
        /// Проверка кода верификации email по email
        /// </summary>
        /// <param name="email">Проверяемый email</param>
        /// <param name="code">Код email проверяемого email</param>
        /// <returns>Результат проверки кода</returns>
        void ValidEmailCode(string email, string code);
        
        /// <summary>
        /// Запрос на проверку подленности номера телефона пользователю по смс
        /// </summary>
        /// <param name="phone">Проверяемый номер телефона</param>
        /// <param name="validatingUser">Необходимость проверки пользоватля по смс</param>
        /// <returns>Результат запуска проверки подленности номера телефона по смс</returns>
        Task RequestCheckPhoneAsync(string phone, bool validatingUser = true);

        /// <summary>
        /// Проверка кода верификации номера телефона по смс
        /// </summary>
        /// <param name="phone">Проверяемый номер телефона</param>
        /// <param name="code">Код смс проверяемого номера телефона</param>
        /// <returns>Результат проверки кода</returns>
        void ValidSmsCode(string phone, string code);

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="model">Данные нового пользователя</param>
        Task<Guid> RegisterAsync(RegisterUserModel model);

        /// <summary>
        /// Обновление данных пользователя
        /// </summary>
        /// <param name="model">Данные изменения пользователя</param>
        Task UpdateUserAsync(UpdateUserModel model);

        /// <summary>
        /// Изменение email пользователя
        /// </summary>
        /// <param name="user">Изменяемый пользователь</param>
        /// <param name="email">email пользователя</param>
        /// <param name="code">Код подтверждения email</param>
        Task ChangeEmailAsync(TUser user, string email, string code);

        /// <summary>
        /// Изменение номера телефона пользователя
        /// </summary>
        /// <param name="user">Изменяемый пользователь</param>
        /// <param name="phoneNumber">Номер телефона пользователя</param>
        /// <param name="code">Код подтверждения номера телефона</param>
        Task ChangePhoneAsync(TUser user, string phoneNumber, string code);

        /// <summary>
        /// Изменение пароля пользователя
        /// </summary>
        /// <param name="idext">Внешний id пользователя портала</param>
        /// <param name="password">Новый пароль пользователя</param>
        Task UpdatePasswordAsync(Guid idext, string password);

        /// <summary>
        /// Восстановление пароля по email
        /// </summary>
        /// <param name="email">Email восcтанвовления</param>
        Task RestorePasswordByEmailAsync(string email);

        /// <summary>
        /// Запрос на восстановление пароля по смс
        /// </summary>
        /// <param name="phoneNumber">Номер телефона для востанвовления</param>
        Task RepairPasswordBySmsAsync(string phoneNumber);

        /// <summary>
        /// Проверка кода восстановления пароля по смс
        /// </summary>
        /// <param name="phoneNumber">Номер телефона для восстанвовления</param>
        /// <param name="code">Код смс восстановления пароля</param>
        void ValidSmsCodeChangePassword(string phoneNumber, string code);

        /// <summary>
        /// Изменение пароля по смс
        /// </summary>
        /// <param name="phoneNumber">Номер телефона для восстанвовления</param>
        /// <param name="code">Код смс восстановления пароля</param>
        /// <param name="password">Новый пароль</param>
        Task ChangePasswordBySmsAsync(string phoneNumber, string code, string password);
    }
}