using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Iserv.AccountService.Core.Portal;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Models;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Iserv.AccountService.Core.Portal.DtoItems.Profile;
using System.Linq;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Helpers;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    /// <summary>
    /// Сервис управления с акаунтом пользователя
    /// </summary>
    public class AccountService<TUser, TKey>
        where TUser : UserIdentity, new()
        where TKey : IEquatable<TKey>
    {
        private readonly string _actionName = "pwd";
        private readonly UserManager<TUser> _userManager;
        private readonly IPortalRegistrationService _portalRegistrationService;
        private readonly IConfirmBySMSService _confirmBySMSService;
        private readonly string _templateMessage;

        public AccountService(UserManager<TUser> userManager, IPortalRegistrationService portalRegistrationService, IConfirmBySMSService confirmBySMSService, string templateMessage)
        {
            _userManager = userManager;
            _portalRegistrationService = portalRegistrationService;
            _confirmBySMSService = confirmBySMSService;
            _templateMessage = templateMessage;
        }

        public async Task RegisterAsync(RegistrUserModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.PhoneNumber))
                throw new ValidationException($"Email and phone is not defined");
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var vEmail = new EmailAddressAttribute();
                if (!vEmail.IsValid(model.Email))
                    throw new ValidationException($"Invalid email. {vEmail.ErrorMessage}");
            }
            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                var vPhone = new PhoneAttribute();
                if (!vPhone.IsValid(model.PhoneNumber))
                    throw new ValidationException($"Invalid phone number. {vPhone.ErrorMessage}");
            }

            var user = new TUser
            {
                UserName = string.IsNullOrWhiteSpace(model.Email) ? model.PhoneNumber : model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                ValidSMSCodeChangePassword(model.PhoneNumber, model.SmsCode);
            }

            var result = await _portalRegistrationService.RegistrateAsync(new PortalRegistrationData()
            {
                Email = user.Email,
                Phone = user.PhoneNumber,
                Password = model.Password,
            });
            if (result.IsError)
                throw new PortalException(result.Message);
        }

        public async Task UpdateUserAsync(TUser user, UserModel model)
        {
            if (user == null)
            {
                throw new ValidationException("UserNotFound");
            }

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException(string.Join(". ", setEmailResult.Errors.Select(e => e.Description)));
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException(string.Join(". ", setPhoneResult.Errors.Select(e => e.Description)));
                }
            }

            var claimsNew = OpenIdClaimHelpers.GetClaims<TKey>(model.Fields);
            var claimsOld = (await _userManager.GetClaimsAsync(user)).ToArray();
            var claimsToRemove = OpenIdClaimHelpers.ExtractClaimsToRemove(claimsOld, claimsNew);
            var claimsToAdd = OpenIdClaimHelpers.ExtractClaimsToAdd(claimsOld, claimsNew);
            var claimsToReplace = OpenIdClaimHelpers.ExtractClaimsToReplace(claimsOld, claimsNew);

            await _userManager.RemoveClaimsAsync(user, claimsToRemove);
            await _userManager.AddClaimsAsync(user, claimsToAdd);

            foreach (var pair in claimsToReplace)
            {
                await _userManager.ReplaceClaimAsync(user, pair.Item1, pair.Item2);
            }
        }

        /// <summary>
        /// Восстановление пароля по email
        /// </summary>
        /// <param name="email">Email востанвовления</param>
        /// <returns>Результат запуска востановления пароля</returns>
        public async Task RestorePasswordByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email is null");
            var vEmail = new EmailAddressAttribute();
            if (!vEmail.IsValid(email))
                throw new ValidationException(vEmail.ErrorMessage);
            var result = await _portalRegistrationService.RestorePasswordByEmailAsync(email);
            if (result.IsError)
                throw new PortalException(result.Message);
        }

        /// <summary>
        /// Запрос на восстановление пароля по смс
        /// </summary>
        /// <param name="phone">Номер телефона для востанвовления</param>
        /// <returns>Результат запуска востановления пароля</returns>
        public async Task RepairPasswordBySMSAsync(string phone)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
            if (user == null)
                throw new ValidationException($"The user with the phone number {phone} was not found");
            if (user.Idext == null)
                throw new ValidationException($"Idext is not defined");
            await _confirmBySMSService.ConfirmAsync(phone, _actionName, _templateMessage);
        }

        /// <summary>
        /// Проверка кода восстановления пароля по смс
        /// </summary>
        /// <param name="phone">Номер телефона для восстанвовления</param>
        /// <param name="code">Код смс восстановления пароля</param>
        /// <returns>Результат запуска восстановления пароля</returns>
        public void ValidSMSCodeChangePassword(string phone, string code)
        {
            _confirmBySMSService.ValidCode(phone, _actionName, code);
        }

        /// <summary>
        /// Изменение пароля по смс
        /// </summary>
        /// <param name="phone">Номер телефона для восстанвовления</param>
        /// <param name="code">Код смс восстановления пароля</param>
        /// <param name="password">Новый пароль</param>
        /// <returns>Результат запуска восстановления пароля</returns>
        public async Task ChangePasswordBySMSAsync(string phone, string code, string password)
        {
            ValidSMSCodeChangePassword(phone, code);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
            if (user == null)
                throw new ValidationException($"The user with the phone number {phone} was not found");
            if (user.Idext == null)
                throw new ValidationException($"Idext is not defined");
            var result = await _portalRegistrationService.SetPasswordAsync(user.Idext, password);
            if (result.IsError)
                throw new PortalException(result.Message);
        }
    }
}