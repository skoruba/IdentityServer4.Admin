using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Iserv.IdentityServer4.BusinessLogic.Models;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using System.Linq;
using Iserv.IdentityServer4.BusinessLogic.Helpers;
using Iserv.IdentityServer4.BusinessLogic.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Web;

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
        private readonly string _actionNamePhone = "phone";
        private readonly string _actionNamePwd = "pwd";
        private readonly string _txtConfirmEmailTitle = "Подтвердите ваш адрес электронной почты";
        private readonly string _txtConfirmEmailBody = "Пожалуйста, подтвердите вашу учетную запись <a href='{0}'>нажатием</a>";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<TUser> _userManager;
        private readonly TIdentityDbContext _dbContext;
        private readonly IPortalService _portalService;
        private readonly IEmailSender _emailSender;
        private readonly IConfirmBySMSService _confirmBySMSService;
        private readonly MessageTemplates _messageTemplates;
        private readonly ILogger<AccountService<TIdentityDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>> _logger;

        public AccountService(TIdentityDbContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<TUser> userManager,
            IPortalService portalService, IEmailSender emailSender, IConfirmBySMSService confirmBySMSService, MessageTemplates messageTemplates,
            ILogger<AccountService<TIdentityDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>> logger)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _portalService = portalService;
            _emailSender = emailSender;
            _confirmBySMSService = confirmBySMSService;
            _messageTemplates = messageTemplates;
            _logger = logger;
        }

        public async Task<TUser> FindByPhoneAsync(string phone)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
        }

        public async Task<TUser> FindByIdextAsync(Guid idext)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Idext == idext);
        }

        public async Task CreateUserAsync(Guid idext, string password)
        {
            var resultPortalData = await _portalService.GetUserAsync(idext);
            if (resultPortalData.IsError)
                return;
            if (!resultPortalData.Value.ContainsKey("idext"))
                resultPortalData.Value.Add("idext", idext.ToString());
            var user = OpenIdClaimHelpers.GetUserBase<TUser, TKey>(resultPortalData.Value);
            var claimsNew = OpenIdClaimHelpers.GetClaims<TKey>(resultPortalData.Value);
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                _logger.LogError(string.Join("\n", result.Errors.Select(er => er.Description)));
                return;
            }
            result = await _userManager.AddClaimsAsync(user, claimsNew);
            if (!result.Succeeded)
            {
                _logger.LogError(string.Join("\n", result.Errors.Select(er => er.Description)));
            }
        }

        public async Task RequestCheckPhoneAsync(string phone)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
            if (user == null)
                throw new ValidationException($"The user with the phone number {phone} was not found");
            if (user.Idext == null)
                throw new ValidationException($"Idext is not defined");
            await _confirmBySMSService.ConfirmAsync(phone, _actionNamePhone, _messageTemplates.CheckPhoneNumberSms);
        }

        public void ValidSMSCodeCheckPhone(string phone, string code)
        {
            _confirmBySMSService.ValidCode(phone, _actionNamePhone, code);
        }

        private void validEmail(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var vEmail = new EmailAddressAttribute();
                if (!vEmail.IsValid(email))
                    throw new ValidationException($"Invalid email. {vEmail.ErrorMessage}");
            }
        }

        private void validPhone(string phone)
        {
            if (!string.IsNullOrWhiteSpace(phone))
            {
                var vPhone = new PhoneAttribute();
                if (!vPhone.IsValid(phone))
                    throw new ValidationException($"Invalid phone number. {vPhone.ErrorMessage}");
            }
        }

        private async Task sendEmailConfirmAsync(TUser user)
        {
            if (user != null)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = string.Format("{0}://{1}/{2}", _httpContextAccessor.HttpContext.Request.Scheme, _httpContextAccessor.HttpContext.Request.Host, $"Account/ConfirmEmail?userId={user.Id}&code={HttpUtility.UrlEncode(code)}");
                await _emailSender.SendEmailAsync(user.Email, _txtConfirmEmailTitle, string.Format(_txtConfirmEmailBody, HtmlEncoder.Default.Encode(callbackUrl)));
            }
        }

        public async Task<Guid> RegisterAsync(RegistrUserModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.PhoneNumber))
                throw new ValidationException($"Email and phone is not defined");
            validEmail(model.Email);
            validPhone(model.PhoneNumber);
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
                ValidSMSCodeCheckPhone(model.PhoneNumber, model.SmsCode);
            }

            var resultPortalRegistr = await _portalService.RegistrateAsync(new PortalRegistrationData()
            {
                Email = model.Email,
                Phone = model.PhoneNumber,
                Password = model.Password,
            });

            if (resultPortalRegistr.IsError)
                throw new ValidationException(resultPortalRegistr.Message);
            await CreateUserAsync(resultPortalRegistr.Value, model.Password);

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var user = await FindByIdextAsync(resultPortalRegistr.Value);
                await sendEmailConfirmAsync(user);
            }

            return resultPortalRegistr.Value;
        }

        public async Task UpdateUserAsync(TUser user, Dictionary<string, object> values, IEnumerable<FileModel> files)
        {
            if (user == null)
                throw new ValidationException("UserNotFound");

            var claimsNew = OpenIdClaimHelpers.GetClaims<TKey>(values);
            var claimsOld = (await _userManager.GetClaimsAsync(user)).ToArray();
            var claimsToRemove = OpenIdClaimHelpers.ExtractClaimsToRemove(claimsOld, claimsNew);
            var claimsToAdd = OpenIdClaimHelpers.ExtractClaimsToAdd(claimsOld, claimsNew);
            var claimsToReplace = OpenIdClaimHelpers.ExtractClaimsToReplace(claimsOld, claimsNew);

            await _portalService.UpdateUserAsync(user.Idext, claimsNew.ToDictionary(c => c.Type, c => c.Value as object), files);

            await _userManager.RemoveClaimsAsync(user, claimsToRemove);
            await _userManager.AddClaimsAsync(user, claimsToAdd);

            foreach (var pair in claimsToReplace)
            {
                await _userManager.ReplaceClaimAsync(user, pair.Item1, pair.Item2);
            }
        }

        public async Task ChangeEmailAsync(TUser user, string email)
        {
            if (user == null)
                throw new ValidationException("UserNotFound");
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email not specified");
            validEmail(email);
            if (user.Email == email)
            {
                throw new ValidationException("Старый email совпадает со старым");
            }
            var setEmailResult = await _userManager.SetEmailAsync(user, email);
            if (!setEmailResult.Succeeded)
            {
                throw new ApplicationException(string.Join(". ", setEmailResult.Errors.Select(e => e.Description)));
            }
            await sendEmailConfirmAsync(user);
        }

        public async Task ChangePhoneAsync(TUser user, string phoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, phoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                throw new ApplicationException(string.Join(". ", setPhoneResult.Errors.Select(e => e.Description)));
            }
        }

        public async Task RestorePasswordByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email is null");
            var vEmail = new EmailAddressAttribute();
            if (!vEmail.IsValid(email))
                throw new ValidationException(vEmail.ErrorMessage);
            //var result = await _portalRegistrationService.RestorePasswordByEmailAsync(email);
            //if (result.IsError)
            //throw new PortalException(result.Message);
        }

        public async Task RepairPasswordBySMSAsync(string phone)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
            if (user == null)
                throw new ValidationException($"The user with the phone number {phone} was not found");
            if (user.Idext == null)
                throw new ValidationException($"Idext is not defined");
            await _confirmBySMSService.ConfirmAsync(phone, _actionNamePwd, _messageTemplates.RepairPasswordSms);
        }

        public void ValidSMSCodeChangePassword(string phone, string code)
        {
            _confirmBySMSService.ValidCode(phone, _actionNamePwd, code);
        }

        public async Task ChangePasswordBySMSAsync(string phone, string code, string password)
        {
            ValidSMSCodeChangePassword(phone, code);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
            if (user == null)
                throw new ValidationException($"The user with the phone number {phone} was not found");
            if (user.Idext == null)
                throw new ValidationException($"Idext is not defined");
            //var result = await _portalRegistrationService.SetPasswordAsync(user.Idext, password);
            //if (result.IsError)
            //throw new PortalException(result.Message);
        }
    }
}