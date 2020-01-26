using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Models;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class ExportIdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> : IExportIdentityService where TUserDto : UserDto<TUserDtoKey>
        where TRoleDto : RoleDto<TRoleDtoKey>
        where TUser : IdentityUser<TKey>
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

        public ExportIdentityService(IIdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> identityService)
        {
            _identityService = identityService;
        }

        private void validNotNullField(Dictionary<string, object> item, string fieldName)
        {
            if (!item.ContainsKey(fieldName))
                throw new UserFriendlyErrorPageException($"Field '${fieldName}' is not defined");
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
                if (item == null)
                    throw new UserFriendlyErrorPageException("Data is incorrect");
                var user = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.ToString());
                var id = Guid.Empty;
                var password = string.Empty;
                var email = string.Empty;
                var phone = string.Empty;
                var fieldId = "id";
                var fieldPassword = "password";
                var fieldEmail = "e_mail";
                var fieldPhone = "phone_number";

                validNotNullField(user, fieldId);
                if (!Guid.TryParse(user[fieldId]?.ToString(), out id))
                    throw new UserFriendlyErrorPageException($"Invalid field '${fieldId}'");

                validNotNullField(user, fieldPassword);
                password = user[fieldPassword]?.ToString();
                if (string.IsNullOrWhiteSpace(password))
                    throw new UserFriendlyErrorPageException($"Invalid field '${fieldPassword}'");

                if (user.ContainsKey(fieldEmail))
                    email = user[fieldEmail]?.ToString();
                if (user.ContainsKey(fieldPhone))
                    phone = user[fieldPhone]?.ToString();
                if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(phone))
                    throw new UserFriendlyErrorPageException($"Fields '${fieldEmail}' and '${fieldPhone}' is not defined");
                if (!string.IsNullOrWhiteSpace(email))
                {
                    var vEmail = new EmailAddressAttribute();
                    if (!vEmail.IsValid(email))
                        throw new ValidationException($"Invalid field '${fieldEmail}'. ${vEmail.ErrorMessage}");
                }
                if (!string.IsNullOrWhiteSpace(phone))
                {
                    var vPhone = new PhoneAttribute();
                    if (!vPhone.IsValid(phone))
                        throw new ValidationException($"Invalid field '${fieldPhone}'. ${vPhone.ErrorMessage}");
                }

            }
        }
    }
}