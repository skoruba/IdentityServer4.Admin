using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Iserv.IdentityServer4.BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Iserv.IdentityServer4.BusinessLogic.Helpers;
using Iserv.IdentityServer4.BusinessLogic.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Web;
using Iserv.IdentityServer4.BusinessLogic.ExceptionHandling;

namespace Iserv.IdentityServer4.BusinessLogic.Services
{
    public class AccountService<TIdentityDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IAccountService<TUser, TKey>
        where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : UserIdentity<TKey>, new()
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
    {
        private const string IdentityServerCode = "idt";
        private const string ActionNameEmail = "email";
        private const string ActionNamePhone = "phone";
        private const string ActionNamePwd = "pwd";
        private const string TxtConfirmEmailTitle = "Подтвердите ваш адрес электронной почты";
        private const string TxtConfirmEmailBody = "Пожалуйста, подтвердите вашу учетную запись <a href='{0}'>нажатием</a>";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<TUser> _userManager;
        private readonly TIdentityDbContext _dbContext;
        private readonly IPortalService _portalService;
        private readonly IEmailSender _emailSender;
        private readonly IConfirmService _confirmService;
        private readonly MessageTemplates _messageTemplates;

        private readonly ILogger<AccountService<TIdentityDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>> _logger;

        public AccountService(TIdentityDbContext dbContext, IHttpContextAccessor httpContextAccessor,
            UserManager<TUser> userManager,
            IPortalService portalService, IEmailSender emailSender, IConfirmService confirmService,
            MessageTemplates messageTemplates,
            ILogger<AccountService<TIdentityDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>> logger)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _portalService = portalService;
            _emailSender = emailSender;
            _confirmService = confirmService;
            _messageTemplates = messageTemplates;
            _logger = logger;
        }

        private static void ValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return;
            var vEmail = new EmailAddressAttribute();
            if (!vEmail.IsValid(email))
                throw new ValidationException($"Invalid email. {string.Format(vEmail.ErrorMessage, email)}");
        }

        private static void ValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return;
            var vPhone = new PhoneAttribute();
            if (!vPhone.IsValid(phone))
                throw new ValidationException($"Invalid phone number. {string.Format(vPhone.ErrorMessage, phone)}");
        }

        private async Task SendEmailConfirmAsync(TUser user)
        {
            if (user != null)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl =
                    $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/Account/ConfirmEmail?userId={user.Id}&code={HttpUtility.UrlEncode(code)}";
                await _emailSender.SendEmailAsync(user.Email, TxtConfirmEmailTitle, string.Format(TxtConfirmEmailBody, HtmlEncoder.Default.Encode(callbackUrl)));
            }
        }

        public async Task<TUser> FindByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<TUser> FindByPhoneAsync(string phone)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
        }

        public async Task<TUser> FindByIdextAsync(Guid idext)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Idext == idext);
        }

        public async Task<Dictionary<string, string>> GetExtraFieldsAsync(TUser user)
        {
            return (await _userManager.GetClaimsAsync(user)).ToDictionary(c => c.Type, c => c.Value);
        }

        public async Task<IdentityResult> CreateUserFromPortalAsync(Guid idext, string password)
        {
            var user = await FindByIdextAsync(idext);
            if (user != null)
                return IdentityResult.Failed(new IdentityError() {Code = "valid", Description = $"User with 'idext' '{idext}' already exists"});
            var resultPortalData = await _portalService.GetUserAsync(idext);
            if (resultPortalData.IsError)
                return IdentityResult.Failed(new IdentityError() {Code = PortalService.PortalCode, Description = resultPortalData.Message});
            if (!resultPortalData.Value.ContainsKey("idext"))
                resultPortalData.Value.Add("idext", idext.ToString());
            var userNew = UserClaimsHelpers.GetUserBase<TUser, TKey>(resultPortalData.Value);
            var claimsNew = UserClaimsHelpers.GetClaims<TKey>(resultPortalData.Value);
            var result = await _userManager.CreateAsync(userNew, password);
            if (!result.Succeeded)
            {
                _logger.LogError(string.Join("\n", result.Errors.Select(er => er.Description)));
                return result;
            }

            result = await _userManager.AddClaimsAsync(userNew, claimsNew);
            if (!result.Succeeded)
            {
                _logger.LogError(string.Join("\n", result.Errors.Select(er => er.Description)));
                return result;
            }

            return result;
        }

        public async Task<IdentityResult> UpdateUserFromPortalAsync(Guid idext)
        {
            var user = await FindByIdextAsync(idext);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError() {Code = IdentityServerCode, Description = $"User with external id = '{idext}' not found"});
            }

            var resultPortalData = await _portalService.GetUserAsync(idext);
            if (resultPortalData.IsError)
                return IdentityResult.Failed(new IdentityError() {Code = PortalService.PortalCode, Description = resultPortalData.Message});
            if (!resultPortalData.Value.ContainsKey("idext"))
                resultPortalData.Value.Add("idext", idext.ToString());

            var userBase = UserClaimsHelpers.GetUserBase<TUser, TKey>(resultPortalData.Value);
            user.Email = userBase.Email;
            user.EmailConfirmed = userBase.EmailConfirmed;
            user.PhoneNumber = userBase.PhoneNumber;
            user.PhoneNumberConfirmed = userBase.PhoneNumberConfirmed;
            var claimsNew = UserClaimsHelpers.GetClaims<TKey>(resultPortalData.Value);
            var claimsOld = (await _userManager.GetClaimsAsync(user)).ToArray();
            var claimsToRemove = UserClaimsHelpers.ExtractClaimsToRemove(claimsOld, claimsNew);
            var claimsToAdd = UserClaimsHelpers.ExtractClaimsToAdd(claimsOld, claimsNew);
            var claimsToReplace = UserClaimsHelpers.ExtractClaimsToReplace(claimsOld, claimsNew);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError(string.Join("\n", result.Errors.Select(er => er.Description)));
                return result;
            }

            result = await _userManager.RemoveClaimsAsync(user, claimsToRemove);
            if (!result.Succeeded)
            {
                _logger.LogError(string.Join("\n", result.Errors.Select(er => er.Description)));
                return result;
            }

            result = await _userManager.AddClaimsAsync(user, claimsToAdd);
            if (!result.Succeeded)
            {
                _logger.LogError(string.Join("\n", result.Errors.Select(er => er.Description)));
                return result;
            }

            foreach (var pair in claimsToReplace)
            {
                result = await _userManager.ReplaceClaimAsync(user, pair.Item1, pair.Item2);
                if (result.Succeeded) continue;
                _logger.LogError(string.Join("\n", result.Errors.Select(er => er.Description)));
                return result;
            }

            return result;
        }

        public async Task RequestCheckEmailAsync(string email, bool validatingUser)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email number not specified");
            ValidEmail(email);
            if (validatingUser)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                    throw new ValidationException($"The user with the email {email} was not found");
                if (user.Idext == Guid.Empty)
                    throw new ValidationException($"Idext is not defined");
            }

            await _confirmService.ConfirmEmailAsync(email, ActionNameEmail, _messageTemplates.CheckEmailTitle, _messageTemplates.CheckEmail);
        }

        public void ValidEmailCode(string email, string code)
        {
            _confirmService.ValidEmailCode(email, ActionNameEmail, code);
        }

        public async Task RequestCheckPhoneAsync(string phone, bool validatingUser)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ValidationException("Phone number not specified");
            var vPhone = new PhoneAttribute();
            if (!vPhone.IsValid(phone))
                throw new ValidationException($"Invalid phone number. {string.Format(vPhone.ErrorMessage, phone)}");
            if (validatingUser)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
                if (user == null)
                    throw new ValidationException($"The user with the phone number {phone} was not found");
                if (user.Idext == Guid.Empty)
                    throw new ValidationException($"Idext is not defined");
            }

            await _confirmService.ConfirmSmsAsync(phone, ActionNamePhone, _messageTemplates.CheckPhoneNumberSms);
        }

        public void ValidSmsCode(string phone, string code)
        {
            _confirmService.ValidSmsCode(phone, ActionNamePhone, code);
        }

        public async Task<Guid> RegisterAsync(RegisterUserModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
                throw new ValidationException($"Password not specified");
            if (string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.PhoneNumber))
                throw new ValidationException($"Email and phone is not defined");
            ValidEmail(model.Email);
            ValidPhone(model.PhoneNumber);
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var userLocal = await _userManager.FindByEmailAsync(model.Email);
                if (userLocal != null)
                    throw new ValidationException($"Пользователь с почтой '{model.Email}' уже существует");
            }

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                var userLocal = await FindByPhoneAsync(model.PhoneNumber);
                if (userLocal != null)
                    throw new ValidationException($"Пользователь с номером телефона '{model.PhoneNumber}' уже существует");
            }

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                ValidSmsCode(model.PhoneNumber, model.SmsCode);
            }

            var portalModel = new PortalRegistrationData()
            {
                Email = model.Email,
                Phone = model.PhoneNumber,
                Password = model.Password,
            };
            if (string.IsNullOrWhiteSpace(model.FirstName)) portalModel.FirstName = model.FirstName;
            if (string.IsNullOrWhiteSpace(model.LastName)) portalModel.LastName = model.LastName;
            if (string.IsNullOrWhiteSpace(model.MiddleName)) portalModel.MiddleName = model.MiddleName;
            var portalResult = await _portalService.RegisterAsync(portalModel);
            if (portalResult.IsError)
                throw new ValidationException(portalResult.Message);
            if (!(await CreateUserFromPortalAsync(portalResult.Value, model.Password)).Succeeded)
                throw new ValidationException(portalResult.Message);
            return portalResult.Value;
        }

        public async Task UpdateUserAsync(UpdateUserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                throw new ValidationException($"User with id = '{model.Id}' not found");
            }

            var claimsNew = UserClaimsHelpers.GetClaims<TKey>(model.Values);
            var values = claimsNew?.ToDictionary(c => c.Type, c => c.Value as object) ?? new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(model.Email) && user.Email != model.Email)
            {
                ValidEmail(model.Email);
                values.Add("email", model.Email);
            }

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber) && user.PhoneNumber != model.PhoneNumber)
            {
                ValidPhone(model.PhoneNumber);
                values.Add("phone", model.PhoneNumber);
                // ValidSmsCode(model.PhoneNumber, model.SmsCode);
            }

            var portalResult = await _portalService.UpdateUserAsync(user.Idext, values, model.Files);
            if (portalResult.IsError)
                throw new PortalException(portalResult.Message);
            await UpdateUserFromPortalAsync(user.Idext);
        }

        public async Task ChangeEmailAsync(TUser user, string email, string code)
        {
            if (user == null)
                throw new ValidationException("UserNotFound");
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email not specified");
            ValidEmail(email);
            if (user.Email == email)
            {
                throw new ValidationException("The old email matches the old one");
            }

            ValidEmailCode(email, code);
            var portalResult = await _portalService.UpdateUserAsync(user.Idext, new Dictionary<string, object>() {{"email", email}});
            if (portalResult.IsError)
                throw new ValidationException(portalResult.Message);
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, email);
            var setEmailResult = await _userManager.ChangeEmailAsync(user, email, token);
            if (!setEmailResult.Succeeded)
            {
                throw new ApplicationException(setEmailResult.Errors != null ? string.Join(". ", setEmailResult.Errors.Select(e => e.Description)) : null);
            }
        }

        public async Task ChangePhoneAsync(TUser user, string phoneNumber, string code)
        {
            if (user == null)
                throw new ValidationException("UserNotFound");
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ValidationException("Phone number not specified");
            ValidPhone(phoneNumber);
            if (user.PhoneNumber == phoneNumber)
            {
                throw new ValidationException("The old phone number matches the old one");
            }

            ValidSmsCode(phoneNumber, code);
            var portalResult = await _portalService.UpdateUserAsync(user.Idext, new Dictionary<string, object>() {{"phone", phoneNumber}});
            if (portalResult.IsError)
                throw new ValidationException(portalResult.Message);
            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            var setPhoneResult = await _userManager.ChangePhoneNumberAsync(user, phoneNumber, token);
            if (!setPhoneResult.Succeeded)
            {
                throw new ApplicationException(setPhoneResult.Errors != null ? string.Join(". ", setPhoneResult.Errors.Select(e => e.Description)) : null);
            }
        }

        public async Task UpdatePasswordAsync(Guid idext, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ValidationException("Password number not specified");
            var user = await FindByIdextAsync(idext);
            if (user == null)
                throw new ValidationException($"User with external id = '{idext}' not found");
            var result = await _portalService.UpdatePasswordAsync(idext, password);
            if (result.IsError)
                throw new PortalException(result.Message);
        }

        public async Task RestorePasswordByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email is null");
            var vEmail = new EmailAddressAttribute();
            if (!vEmail.IsValid(email))
                throw new ValidationException(vEmail.ErrorMessage);
            var user = await FindByEmailAsync(email);
            if (user == null)
                throw new ValidationException($"User with email = '{email}' not found");
            var result = await _portalService.RestorePasswordByEmailAsync(email);
            if (result.IsError)
                throw new PortalException(result.Message);
        }

        public async Task RepairPasswordBySmsAsync(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ValidationException("Phone number not specified");
            ValidPhone(phoneNumber);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (user == null)
                throw new ValidationException($"The user with the phone number {phoneNumber} was not found");
            if (user.Idext == null)
                throw new ValidationException($"Idext is not defined");
            await _confirmService.ConfirmSmsAsync(phoneNumber, ActionNamePwd, _messageTemplates.RepairPasswordSms);
        }

        public void ValidSmsCodeChangePassword(string phoneNumber, string code)
        {
            _confirmService.ValidSmsCode(phoneNumber, ActionNamePwd, code);
        }

        public async Task ChangePasswordBySmsAsync(string phoneNumber, string code, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ValidationException("Password number not specified");
            ValidSmsCodeChangePassword(phoneNumber, code);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (user == null)
                throw new ValidationException($"The user with the phone number {phoneNumber} was not found");
            if (user.Idext == null)
                throw new ValidationException($"Idext is not defined");
            await UpdatePasswordAsync(user.Idext, password);
        }
    }
}