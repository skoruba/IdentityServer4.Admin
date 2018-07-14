using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IIdentityServiceResources _identityServiceResources;

        public IdentityService(IIdentityRepository identityRepository,
            IIdentityServiceResources identityServiceResources)
        {
            _identityRepository = identityRepository;
            _identityServiceResources = identityServiceResources;
        }

        public async Task<bool> ExistsUserAsync(int userId)
        {
            var exists = await _identityRepository.ExistsUserAsync(userId);
            if (!exists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, userId), _identityServiceResources.UserDoesNotExist().Description);

            return true;
        }

        public async Task<bool> ExistsRoleAsync(int roleId)
        {
            var exists = await _identityRepository.ExistsRoleAsync(roleId);
            if (!exists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.RoleDoesNotExist().Description, roleId), _identityServiceResources.RoleDoesNotExist().Description);

            return true;
        }

        public async Task<UsersDto> GetUsersAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await _identityRepository.GetUsersAsync(search, page, pageSize);
            var usersDto = pagedList.ToModel();

            return usersDto;
        }

        public async Task<RolesDto> GetRolesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await _identityRepository.GetRolesAsync(search, page, pageSize);
            var rolesDto = pagedList.ToModel();

            return rolesDto;
        }

        public async Task<IdentityResult> CreateRoleAsync(RoleDto role)
        {
            var roleEntity = role.ToEntity();
            var identityResult = await _identityRepository.CreateRoleAsync(roleEntity);

            return HandleIdentityError(identityResult, _identityServiceResources.RoleCreateFailed().Description, _identityServiceResources.IdentityErrorKey().Description, role);
        }

        private IdentityResult HandleIdentityError(IdentityResult identityResult, string errorMessage, string errorKey, object model)
        {
            if (!identityResult.Errors.Any()) return identityResult;
            var viewErrorMessages = identityResult.Errors.ToModel();
            throw new UserFriendlyViewException(errorMessage, errorKey, viewErrorMessages, model);
        }

        public async Task<RoleDto> GetRoleAsync(RoleDto role)
        {
            var roleEntity = role.ToEntity();
            var userIdentityRole = await _identityRepository.GetRoleAsync(roleEntity);
            if (userIdentityRole == null) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.RoleDoesNotExist().Description, role.Id), _identityServiceResources.RoleDoesNotExist().Description);
            var roleDto = userIdentityRole.ToModel();

            return roleDto;
        }

        public async Task<List<RoleDto>> GetRolesAsync()
        {
            var roles = await _identityRepository.GetRolesAsync();
            var roleDtos = roles.ToModel();

            return roleDtos;
        }

        public async Task<IdentityResult> UpdateRoleAsync(RoleDto role)
        {
            var userIdentityRole = role.ToEntity();
            var identityResult = await _identityRepository.UpdateRoleAsync(userIdentityRole);

            return HandleIdentityError(identityResult, _identityServiceResources.RoleUpdateFailed().Description, _identityServiceResources.IdentityErrorKey().Description, role);
        }

        public async Task<UserDto> GetUserAsync(UserDto user)
        {
            var userIdentity = user.ToEntity();
            var identity = await _identityRepository.GetUserAsync(userIdentity);
            if (identity == null) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, user.Id), _identityServiceResources.UserDoesNotExist().Description);

            var userDto = identity.ToModel();

            return userDto;
        }

        public async Task<IdentityResult> CreateUserAsync(UserDto user)
        {
            var userIdentity = user.ToEntity();
            var identityResult = await _identityRepository.CreateUserAsync(userIdentity);

            return HandleIdentityError(identityResult, _identityServiceResources.UserCreateFailed().Description, _identityServiceResources.IdentityErrorKey().Description, user);
        }

        public async Task<IdentityResult> UpdateUserAsync(UserDto user)
        {
            var userIdentity = user.ToEntity();
            var identityResult = await _identityRepository.UpdateUserAsync(userIdentity);

            return HandleIdentityError(identityResult, _identityServiceResources.UserUpdateFailed().Description, _identityServiceResources.IdentityErrorKey().Description, user);
        }

        public async Task<IdentityResult> DeleteUserAsync(UserDto user)
        {
            var userIdentity = user.ToEntity();
            var identityResult = await _identityRepository.DeleteUserAsync(userIdentity);

            return HandleIdentityError(identityResult, _identityServiceResources.UserDeleteFailed().Description, _identityServiceResources.IdentityErrorKey().Description, user);
        }

        public async Task<IdentityResult> CreateUserRoleAsync(UserRolesDto role)
        {
            var identityResult = await _identityRepository.CreateUserRoleAsync(role.UserId, role.RoleId);

            if (!identityResult.Errors.Any()) return identityResult;

            var userRolesDto = await BuildUserRolesViewModel(role.UserId, 1);
            return HandleIdentityError(identityResult, _identityServiceResources.UserRoleCreateFailed().Description, _identityServiceResources.IdentityErrorKey().Description, userRolesDto);
        }

        public async Task<UserRolesDto> BuildUserRolesViewModel(int id, int? page)
        {
            var roles = await GetRolesAsync();
            var userRoles = await GetUserRolesAsync(id, page ?? 1);
            userRoles.UserId = id;
            userRoles.RolesList = roles.Select(x => new SelectItem(x.Id.ToString(), x.Name)).ToList();

            return userRoles;
        }

        public async Task<UserRolesDto> GetUserRolesAsync(int userId, int page = 1, int pageSize = 10)
        {
            var userExists = await _identityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, userId), _identityServiceResources.UserDoesNotExist().Description);

            var userIdentityRoles = await _identityRepository.GetUserRolesAsync(userId, page, pageSize);
            var roleDtos = userIdentityRoles.MapToModel();

            return roleDtos;
        }

        public async Task<IdentityResult> DeleteUserRoleAsync(UserRolesDto role)
        {
            var identityResult = await _identityRepository.DeleteUserRoleAsync(role.UserId, role.RoleId);

            return HandleIdentityError(identityResult, _identityServiceResources.UserRoleDeleteFailed().Description, _identityServiceResources.IdentityErrorKey().Description, role);
        }

        public async Task<UserClaimsDto> GetUserClaimsAsync(int userId, int page = 1, int pageSize = 10)
        {
            var userExists = await _identityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, userId), _identityServiceResources.UserDoesNotExist().Description);

            PagedList<UserIdentityUserClaim> identityUserClaims = await _identityRepository.GetUserClaimsAsync(userId, page, pageSize);
            var claimDtos = identityUserClaims.ToModel();

            return claimDtos;
        }

        public async Task<UserClaimsDto> GetUserClaimAsync(int userId, int claimId)
        {
            var userExists = await _identityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, userId), _identityServiceResources.UserDoesNotExist().Description);

            var identityUserClaim = await _identityRepository.GetUserClaimAsync(userId, claimId);
            if (identityUserClaim == null) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserClaimDoesNotExist().Description, userId), _identityServiceResources.UserClaimDoesNotExist().Description);

            var userClaimsDto = identityUserClaim.ToModel();

            return userClaimsDto;
        }

        public async Task<IdentityResult> CreateUserClaimsAsync(UserClaimsDto claimsDto)
        {
            var userIdentityUserClaim = claimsDto.ToEntity();
            var identityResult = await _identityRepository.CreateUserClaimsAsync(userIdentityUserClaim);

            return HandleIdentityError(identityResult, _identityServiceResources.UserClaimsCreateFailed().Description, _identityServiceResources.IdentityErrorKey().Description, claimsDto);
        }

        public async Task<int> DeleteUserClaimsAsync(UserClaimsDto claim)
        {
            return await _identityRepository.DeleteUserClaimsAsync(claim.UserId, claim.ClaimId);
        }

        public async Task<UserProvidersDto> GetUserProvidersAsync(int userId)
        {
            var userExists = await _identityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, userId), _identityServiceResources.UserDoesNotExist().Description);

            var userLoginInfos = await _identityRepository.GetUserProvidersAsync(userId);
            var providersDto = userLoginInfos.ToModel();
            providersDto.UserId = userId;

            return providersDto;
        }

        public async Task<IdentityResult> DeleteUserProvidersAsync(UserProviderDto provider)
        {
            var identityResult = await _identityRepository.DeleteUserProvidersAsync(provider.UserId, provider.ProviderKey, provider.LoginProvider);

            return HandleIdentityError(identityResult, _identityServiceResources.UserProviderDeleteFailed().Description, _identityServiceResources.IdentityErrorKey().Description, provider);
        }

        public async Task<UserProviderDto> GetUserProviderAsync(int userId, string providerKey)
        {
            var userExists = await _identityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, userId), _identityServiceResources.UserDoesNotExist().Description);

            var identityUserLogin = await _identityRepository.GetUserProviderAsync(userId, providerKey);
            if (identityUserLogin == null) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserProviderDoesNotExist().Description, providerKey), _identityServiceResources.UserProviderDoesNotExist().Description);

            var userProviderDto = identityUserLogin.ToModel();

            return userProviderDto;
        }

        public async Task<IdentityResult> UserChangePasswordAsync(UserChangePasswordDto userPassword)
        {
            var userExists = await _identityRepository.ExistsUserAsync(userPassword.UserId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, userPassword.UserId), _identityServiceResources.UserDoesNotExist().Description);

            var identityResult = await _identityRepository.UserChangePasswordAsync(userPassword.UserId, userPassword.Password);

            return HandleIdentityError(identityResult, _identityServiceResources.UserChangePasswordFailed().Description, _identityServiceResources.IdentityErrorKey().Description, userPassword);
        }

        public async Task<IdentityResult> CreateRoleClaimsAsync(RoleClaimsDto claimsDto)
        {
            var identityRoleClaim = claimsDto.ToEntity();
            var identityResult = await _identityRepository.CreateRoleClaimsAsync(identityRoleClaim);

            return HandleIdentityError(identityResult, _identityServiceResources.RoleClaimsCreateFailed().Description, _identityServiceResources.IdentityErrorKey().Description, claimsDto);
        }

        public async Task<RoleClaimsDto> GetRoleClaimsAsync(int roleId, int page = 1, int pageSize = 10)
        {
            var roleExists = await _identityRepository.ExistsRoleAsync(roleId);
            if (!roleExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.RoleDoesNotExist().Description, roleId), _identityServiceResources.RoleDoesNotExist().Description);

            var identityRoleClaims = await _identityRepository.GetRoleClaimsAsync(roleId, page, pageSize);
            var roleClaimDtos = identityRoleClaims.ToModel();

            return roleClaimDtos;
        }

        public async Task<RoleClaimsDto> GetRoleClaimAsync(int roleId, int claimId)
        {
            var roleExists = await _identityRepository.ExistsRoleAsync(roleId);
            if (!roleExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.RoleDoesNotExist().Description, roleId), _identityServiceResources.RoleDoesNotExist().Description);

            var identityRoleClaim = await _identityRepository.GetRoleClaimAsync(roleId, claimId);
            if (identityRoleClaim == null) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.RoleClaimDoesNotExist().Description, claimId), _identityServiceResources.RoleClaimDoesNotExist().Description);

            var roleClaimsDto = identityRoleClaim.ToModel();

            return roleClaimsDto;
        }

        public async Task<int> DeleteRoleClaimsAsync(RoleClaimsDto role)
        {
            return await _identityRepository.DeleteRoleClaimsAsync(role.RoleId, role.ClaimId);
        }

        public async Task<IdentityResult> DeleteRoleAsync(RoleDto role)
        {
            var userIdentityRole = role.ToEntity();
            var identityResult = await _identityRepository.DeleteRoleAsync(userIdentityRole);

            return HandleIdentityError(identityResult, _identityServiceResources.RoleDeleteFailed().Description, _identityServiceResources.IdentityErrorKey().Description, role);
        }
    }
}