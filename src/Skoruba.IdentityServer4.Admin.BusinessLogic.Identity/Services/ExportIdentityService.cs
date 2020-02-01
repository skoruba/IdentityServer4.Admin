using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class ExportIdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> : IExportIdentityService where TUserDto : UserDto<TUserDtoKey>
        where TRoleDto : RoleDto<TRoleDtoKey>
        where TUser : IdentityUser<TKey>, new()
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TUsersDto : UsersDto<TUserDto, TUserDtoKey>
        where TRolesDto : RolesDto<TRoleDto, TRoleDtoKey>
        where TUserRolesDto : UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey>
        where TUserClaimsDto : UserClaimsDto<TUserDtoKey>
        where TUserProviderDto : UserProviderDto<TUserDtoKey>
        where TUserProvidersDto : UserProvidersDto<TUserDtoKey>
        where TUserChangePasswordDto : UserChangePasswordDto<TUserDtoKey>
        where TRoleClaimsDto : RoleClaimsDto<TRoleDtoKey>
    {
        private readonly IIdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> _identityService;
        private readonly UserManager<TUser> _userManager;

        public ExportIdentityService(IIdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> identityService,
            UserManager<TUser> userManager)
        {
            _identityService = identityService;
            _userManager = userManager;
        }

        private void validNotNullField(Dictionary<string, object> item, string fieldName)
        {
            if (!item.ContainsKey(fieldName))
                throw new UserFriendlyErrorPageException($"Field '{fieldName}' is not defined");
        }

        private string getFieldNameFromFields(Dictionary<string, object> item, string[] fieldNames)
        {
            string field = null;
            foreach (var fieldName in fieldNames)
            {
                if (item.ContainsKey(fieldName))
                {
                    field = fieldName;
                    break;
                }
            }
            if (field == null)
                throw new UserFriendlyErrorPageException($"Fields '{string.Join(", ", fieldNames)}' is not defined");
            return field;
        }

        private async Task updateUserAsync(object objUser)
        {
            if (objUser == null)
                throw new UserFriendlyErrorPageException("Data is incorrect");
            var userFields = JsonConvert.DeserializeObject<Dictionary<string, object>>(objUser.ToString());
            TKey id = default;
            var extId = Guid.Empty;
            var login = string.Empty;
            var password = string.Empty;
            var email = string.Empty;
            var phone = string.Empty;
            var fieldId = "id";
            var fieldsExtId = new[] { "idext", "extId" };
            var fieldLogin = "login";
            var fieldPassword = "password";
            var fieldEmail = "e_mail";
            var fieldPhone = "phone_number";

            validNotNullField(userFields, fieldId);
            id = (TKey)Convert.ChangeType(userFields[fieldId], typeof(TKey));
            if (id.Equals(default))
            {
                throw new UserFriendlyErrorPageException($"Invalid field '{fieldId}'");
            }
            userFields.Remove(fieldId);

            var fieldExtId = getFieldNameFromFields(userFields, fieldsExtId);
            if (!Guid.TryParse(userFields[fieldExtId]?.ToString(), out extId))
                throw new UserFriendlyErrorPageException($"Invalid field '{fieldExtId}'");
            if (fieldsExtId[0] != fieldExtId)
            {
                userFields.Remove(fieldExtId);
                userFields.Add(fieldsExtId[0], extId);
            }

            validNotNullField(userFields, fieldPassword);
            password = userFields[fieldPassword]?.ToString();
            if (string.IsNullOrWhiteSpace(password))
                throw new UserFriendlyErrorPageException($"Invalid field '{fieldPassword}'");
            userFields.Remove(fieldPassword);

            if (userFields.ContainsKey(fieldEmail))
            {
                email = userFields[fieldEmail]?.ToString();
                userFields.Remove(fieldEmail);
            }
            if (userFields.ContainsKey(fieldPhone))
            {
                phone = userFields[fieldPhone]?.ToString();
                userFields.Remove(fieldPhone);
            }
            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
                throw new UserFriendlyErrorPageException($"Fields '{fieldEmail}' and '{fieldPhone}' is not defined");

            login = userFields[fieldLogin]?.ToString();
            if (string.IsNullOrWhiteSpace(login))
            {
                login = string.IsNullOrWhiteSpace(email) ? phone : email;
            }
            else
            {
                userFields.Remove(fieldLogin);
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var vEmail = new EmailAddressAttribute();
                if (!vEmail.IsValid(email))
                    throw new UserFriendlyErrorPageException($"Invalid field '{fieldEmail}'. {vEmail.ErrorMessage}");
            }
            if (!string.IsNullOrWhiteSpace(phone))
            {
                var vPhone = new PhoneAttribute();
                if (!vPhone.IsValid(phone))
                    throw new UserFriendlyErrorPageException($"Invalid field '{fieldPhone}'. {vPhone.ErrorMessage}");
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                user = new TUser
                {
                    Id = id,
                    UserName = login,
                    Email = email,
                    PhoneNumber = phone
                };
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    throw new UserFriendlyErrorPageException(string.Join("\n", result.Errors.Select(er => er.Description)));
                await _userManager.AddClaimsAsync(user, userFields.Where(u => u.Value != null).Select(u => new Claim(u.Key, u.Value.ToString())));
            }
            else
            {
                var claims = await _userManager.GetClaimsAsync(user);
            }
        }

        public async Task ImportUsersAsync(string txt)
        {
            if (string.IsNullOrWhiteSpace(txt))
            {
                throw new UserFriendlyErrorPageException("The file is not a configuration");
            }
            var users = JsonConvert.DeserializeObject<dynamic[]>(txt);
            if (users == null)
            {
                throw new UserFriendlyErrorPageException("The file is not a users");
            }
            foreach (object item in users)
            {
                await updateUserAsync(item);
            }
        }
    }
}