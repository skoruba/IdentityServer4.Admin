using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class ExportIdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto> : IExportIdentityService where TUserDto : UserDto<TUserDtoKey>
        where TRoleDto : RoleDto<TRoleDtoKey>
        where TUser : UserIdentity<TKey>, new()
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

        private async Task updateUserAsync(object objUser)
        {
            if (objUser == null)
                throw new UserFriendlyErrorPageException("Data is incorrect");
            var userFields = JsonConvert.DeserializeObject<Dictionary<string, object>>(objUser.ToString());

            string password;
            var userBase = OpenIdClaimHelpers.GetUserBase<TUser, TKey>(userFields, out password);
            var claimsNew = OpenIdClaimHelpers.GetClaims<TKey>(userFields);

            var user = await _userManager.FindByIdAsync(userBase.Id.ToString());
            if (user == null)
            {
                user = new TUser
                {
                    Id = userBase.Id,
                    Idext = userBase.Idext,
                    UserName = userBase.UserName,
                    Email = userBase.Email,
                    PhoneNumber = userBase.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    throw new UserFriendlyErrorPageException(string.Join("\n", result.Errors.Select(er => er.Description)));
                await _userManager.AddClaimsAsync(user, userFields.Where(u => u.Value != null).Select(u => new Claim(u.Key, u.Value.ToString())));
            }
            else
            {
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