using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services
{
    public class IdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, 
        TUserLogin, TRoleClaim, TUserToken,
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>: IIdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole,
        TUserLogin, TRoleClaim, TUserToken,
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>
        where TUserDto : UserDto<TUserDtoKey>
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
        private readonly IIdentityRepository<TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> _identityRepository;
        private readonly IIdentityServiceResources _identityServiceResources;
        private readonly IMapper _mapper;

        public IdentityService(IIdentityRepository<TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> identityRepository,
            IIdentityServiceResources identityServiceResources,
            IMapper mapper)
        {
            _identityRepository = identityRepository;
            _identityServiceResources = identityServiceResources;
            _mapper = mapper;
        }

        public async Task<bool> ExistsUserAsync(string userId)
        {
            var exists = await _identityRepository.ExistsUserAsync(userId);
            if (!exists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, userId), _identityServiceResources.UserDoesNotExist().Description);

            return true;
        }

        public async Task<bool> ExistsRoleAsync(string roleId)
        {
            var exists = await _identityRepository.ExistsRoleAsync(roleId);
            if (!exists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.RoleDoesNotExist().Description, roleId), _identityServiceResources.RoleDoesNotExist().Description);

            return true;
        }

        public async Task<TUsersDto> GetUsersAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await _identityRepository.GetUsersAsync(search, page, pageSize);
            var usersDto = _mapper.Map<TUsersDto>(pagedList);

            return usersDto;
        }

        public async Task<TRolesDto> GetRolesAsync(string search, int page = 1, int pageSize = 10)
        {
            PagedList<TRole> pagedList = await _identityRepository.GetRolesAsync(search, page, pageSize);
            var rolesDto = _mapper.Map<TRolesDto>(pagedList);

            return rolesDto;
        }

        public async Task<(IdentityResult identityResult, TKey roleId)> CreateRoleAsync(TRoleDto role)
        {
            var roleEntity = _mapper.Map<TRole>(role);
            var (identityResult, roleId) = await _identityRepository.CreateRoleAsync(roleEntity);
            var handleIdentityError = HandleIdentityError(identityResult, _identityServiceResources.RoleCreateFailed().Description, _identityServiceResources.IdentityErrorKey().Description, role);

            return (handleIdentityError, roleId);
        }

        private IdentityResult HandleIdentityError(IdentityResult identityResult, string errorMessage, string errorKey, object model)
        {
            if (!identityResult.Errors.Any()) return identityResult;
            var viewErrorMessages = _mapper.Map<List<ViewErrorMessage>>(identityResult.Errors);

            throw new UserFriendlyViewException(errorMessage, errorKey, viewErrorMessages, model);
        }

        public async Task<TRoleDto> GetRoleAsync(string roleId)
        {
            var roleKey = ConvertToKeyFromString(roleId);

            var userIdentityRole = await _identityRepository.GetRoleAsync(roleKey);
            if (userIdentityRole == null) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.RoleDoesNotExist().Description, roleId), _identityServiceResources.RoleDoesNotExist().Description);

            var roleDto = _mapper.Map<TRoleDto>(userIdentityRole);

            return roleDto;
        }

        public async Task<List<TRoleDto>> GetRolesAsync()
        {
            var roles = await _identityRepository.GetRolesAsync();
            var roleDtos = _mapper.Map<List<TRoleDto>>(roles);

            return roleDtos;
        }

        public async Task<(IdentityResult identityResult, TKey roleId)> UpdateRoleAsync(TRoleDto role)
        {
            var userIdentityRole = _mapper.Map<TRole>(role);
            var (identityResult, roleId) = await _identityRepository.UpdateRoleAsync(userIdentityRole);

            var handleIdentityError = HandleIdentityError(identityResult, _identityServiceResources.RoleUpdateFailed().Description, _identityServiceResources.IdentityErrorKey().Description, role);

            return (handleIdentityError, roleId);
        }

        public async Task<TUserDto> GetUserAsync(string userId)
        {
            var identity = await _identityRepository.GetUserAsync(userId);
            if (identity == null) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, userId), _identityServiceResources.UserDoesNotExist().Description);

            var userDto = _mapper.Map<TUserDto>(identity);

            return userDto;
        }

        public async Task<(IdentityResult identityResult, TKey userId)> CreateUserAsync(TUserDto user)
        {
            var userIdentity = _mapper.Map<TUser>(user);
            var (identityResult, userId) = await _identityRepository.CreateUserAsync(userIdentity);
            var handleIdentityError = HandleIdentityError(identityResult, _identityServiceResources.UserCreateFailed().Description, _identityServiceResources.IdentityErrorKey().Description, user);

            return (handleIdentityError, userId);
        }

        /// <summary>
        /// Updates the specified user, but without updating the password hash value
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(IdentityResult identityResult, TKey userId)> UpdateUserAsync(TUserDto user)
        {
            var userIdentity = _mapper.Map<TUser>(user);
            await MapOriginalPasswordHashAsync(userIdentity);

            var (identityResult, userId) = await _identityRepository.UpdateUserAsync(userIdentity);
            var handleIdentityError = HandleIdentityError(identityResult, _identityServiceResources.UserUpdateFailed().Description, _identityServiceResources.IdentityErrorKey().Description, user);

            return (handleIdentityError, userId);
        }

        /// <summary>
        /// Get original password hash and map password hash to user
        /// </summary>
        /// <param name="userIdentity"></param>
        /// <returns></returns>
        private async Task MapOriginalPasswordHashAsync(TUser userIdentity)
        {
            var identity = await _identityRepository.GetUserAsync(userIdentity.Id.ToString());
            userIdentity.PasswordHash = identity.PasswordHash;
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId, TUserDto user)
        {
            var identityResult = await _identityRepository.DeleteUserAsync(userId);

            return HandleIdentityError(identityResult, _identityServiceResources.UserDeleteFailed().Description, _identityServiceResources.IdentityErrorKey().Description, user);
        }

        public async Task<IdentityResult> CreateUserRoleAsync(TUserRolesDto role)
        {
            var identityResult = await _identityRepository.CreateUserRoleAsync(role.UserId.ToString(), role.RoleId.ToString());

            if (!identityResult.Errors.Any()) return identityResult;

            var userRolesDto = await BuildUserRolesViewModel(role.UserId, 1);
            return HandleIdentityError(identityResult, _identityServiceResources.UserRoleCreateFailed().Description, _identityServiceResources.IdentityErrorKey().Description, userRolesDto);
        }

        public async Task<TUserRolesDto> BuildUserRolesViewModel(TUserDtoKey id, int? page)
        {
            var roles = await GetRolesAsync();
            var userRoles = await GetUserRolesAsync(id.ToString(), page ?? 1);
            userRoles.UserId = id;
            userRoles.RolesList = roles.Select(x => new SelectItem(x.Id.ToString(), x.Name)).ToList();

            return userRoles;
        }

        public async Task<TUserRolesDto> GetUserRolesAsync(string userId, int page = 1, int pageSize = 10)
        {
            var userExists = await _identityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, userId), _identityServiceResources.UserDoesNotExist().Description);

            var userIdentityRoles = await _identityRepository.GetUserRolesAsync(userId, page, pageSize);
            var roleDtos = _mapper.Map<TUserRolesDto>(userIdentityRoles);

            var user = await _identityRepository.GetUserAsync(userId);
            roleDtos.UserName = user.UserName;

            return roleDtos;
        }

        public async Task<IdentityResult> DeleteUserRoleAsync(TUserRolesDto role)
        {
            var identityResult = await _identityRepository.DeleteUserRoleAsync(role.UserId.ToString(), role.RoleId.ToString());

            return HandleIdentityError(identityResult, _identityServiceResources.UserRoleDeleteFailed().Description, _identityServiceResources.IdentityErrorKey().Description, role);
        }

        public async Task<TUserClaimsDto> GetUserClaimsAsync(string userId, int page = 1, int pageSize = 10)
        {
            var userExists = await _identityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, userId), _identityServiceResources.UserDoesNotExist().Description);

            var identityUserClaims = await _identityRepository.GetUserClaimsAsync(userId, page, pageSize);
            var claimDtos = _mapper.Map<TUserClaimsDto>(identityUserClaims);

            var user = await _identityRepository.GetUserAsync(userId);
            claimDtos.UserName = user.UserName;

            return claimDtos;
        }

        public async Task<TUserClaimsDto> GetUserClaimAsync(string userId, int claimId)
        {
            var userExists = await _identityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, userId), _identityServiceResources.UserDoesNotExist().Description);

            var identityUserClaim = await _identityRepository.GetUserClaimAsync(userId, claimId);
            if (identityUserClaim == null) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserClaimDoesNotExist().Description, userId), _identityServiceResources.UserClaimDoesNotExist().Description);

            var userClaimsDto = _mapper.Map<TUserClaimsDto>(identityUserClaim);

            return userClaimsDto;
        }

        public async Task<IdentityResult> CreateUserClaimsAsync(TUserClaimsDto claimsDto)
        {
            var userIdentityUserClaim = _mapper.Map<TUserClaim>(claimsDto);
            var identityResult = await _identityRepository.CreateUserClaimsAsync(userIdentityUserClaim);

            return HandleIdentityError(identityResult, _identityServiceResources.UserClaimsCreateFailed().Description, _identityServiceResources.IdentityErrorKey().Description, claimsDto);
        }

        public async Task<int> DeleteUserClaimsAsync(TUserClaimsDto claim)
        {
            return await _identityRepository.DeleteUserClaimsAsync(claim.UserId.ToString(), claim.ClaimId);
        }

        public virtual TUserDtoKey ConvertUserDtoKeyFromString(string id)
        {
            if (id == null)
            {
                return default(TUserDtoKey);
            }
            return (TUserDtoKey)TypeDescriptor.GetConverter(typeof(TUserDtoKey)).ConvertFromInvariantString(id);
        }

        public virtual TKey ConvertToKeyFromString(string id)
        {
            if (id == null)
            {
                return default(TKey);
            }
            return (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(id);
        }

        public async Task<TUserProvidersDto> GetUserProvidersAsync(string userId)
        {
            var userExists = await _identityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, userId), _identityServiceResources.UserDoesNotExist().Description);

            var userLoginInfos = await _identityRepository.GetUserProvidersAsync(userId);
            var providersDto = _mapper.Map<TUserProvidersDto>(userLoginInfos);
            providersDto.UserId = ConvertUserDtoKeyFromString(userId);

            var user = await _identityRepository.GetUserAsync(userId);
            providersDto.UserName = user.UserName;

            return providersDto;
        }

        public async Task<IdentityResult> DeleteUserProvidersAsync(TUserProviderDto provider)
        {
            var identityResult = await _identityRepository.DeleteUserProvidersAsync(provider.UserId.ToString(), provider.ProviderKey, provider.LoginProvider);

            return HandleIdentityError(identityResult, _identityServiceResources.UserProviderDeleteFailed().Description, _identityServiceResources.IdentityErrorKey().Description, provider);
        }

        public async Task<TUserProviderDto> GetUserProviderAsync(string userId, string providerKey)
        {
            var userExists = await _identityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, userId), _identityServiceResources.UserDoesNotExist().Description);

            var identityUserLogin = await _identityRepository.GetUserProviderAsync(userId, providerKey);
            if (identityUserLogin == null) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserProviderDoesNotExist().Description, providerKey), _identityServiceResources.UserProviderDoesNotExist().Description);

            var userProviderDto = _mapper.Map<TUserProviderDto>(identityUserLogin);
            var user = await GetUserAsync(userId);
            userProviderDto.UserName = user.UserName;

            return userProviderDto;
        }

        public async Task<IdentityResult> UserChangePasswordAsync(TUserChangePasswordDto userPassword)
        {
            var userExists = await _identityRepository.ExistsUserAsync(userPassword.UserId.ToString());
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.UserDoesNotExist().Description, userPassword.UserId), _identityServiceResources.UserDoesNotExist().Description);

            var identityResult = await _identityRepository.UserChangePasswordAsync(userPassword.UserId.ToString(), userPassword.Password);

            return HandleIdentityError(identityResult, _identityServiceResources.UserChangePasswordFailed().Description, _identityServiceResources.IdentityErrorKey().Description, userPassword);
        }

        public async Task<IdentityResult> CreateRoleClaimsAsync(TRoleClaimsDto claimsDto)
        {
            var identityRoleClaim = _mapper.Map<TRoleClaim>(claimsDto);
            var identityResult = await _identityRepository.CreateRoleClaimsAsync(identityRoleClaim);

            return HandleIdentityError(identityResult, _identityServiceResources.RoleClaimsCreateFailed().Description, _identityServiceResources.IdentityErrorKey().Description, claimsDto);
        }

        public async Task<TRoleClaimsDto> GetRoleClaimsAsync(string roleId, int page = 1, int pageSize = 10)
        {
            var roleExists = await _identityRepository.ExistsRoleAsync(roleId);
            if (!roleExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.RoleDoesNotExist().Description, roleId), _identityServiceResources.RoleDoesNotExist().Description);

            var identityRoleClaims = await _identityRepository.GetRoleClaimsAsync(roleId, page, pageSize);
            var roleClaimDtos = _mapper.Map<TRoleClaimsDto>(identityRoleClaims);
            var roleDto = await GetRoleAsync(roleId);
            roleClaimDtos.RoleName = roleDto.Name;

            return roleClaimDtos;
        }

        public async Task<TRoleClaimsDto> GetRoleClaimAsync(string roleId, int claimId)
        {
            var roleExists = await _identityRepository.ExistsRoleAsync(roleId);
            if (!roleExists) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.RoleDoesNotExist().Description, roleId), _identityServiceResources.RoleDoesNotExist().Description);

            var identityRoleClaim = await _identityRepository.GetRoleClaimAsync(roleId, claimId);
            if (identityRoleClaim == null) throw new UserFriendlyErrorPageException(string.Format(_identityServiceResources.RoleClaimDoesNotExist().Description, claimId), _identityServiceResources.RoleClaimDoesNotExist().Description);
            var roleClaimsDto = _mapper.Map<TRoleClaimsDto>(identityRoleClaim);
            var roleDto = await GetRoleAsync(roleId);
            roleClaimsDto.RoleName = roleDto.Name;

            return roleClaimsDto;
        }

        public async Task<int> DeleteRoleClaimsAsync(TRoleClaimsDto role)
        {
            return await _identityRepository.DeleteRoleClaimsAsync(role.RoleId.ToString(), role.ClaimId);
        }

        public async Task<IdentityResult> DeleteRoleAsync(TRoleDto role)
        {
            var userIdentityRole = _mapper.Map<TRole>(role);
            var identityResult = await _identityRepository.DeleteRoleAsync(userIdentityRole);

            return HandleIdentityError(identityResult, _identityServiceResources.RoleDeleteFailed().Description, _identityServiceResources.IdentityErrorKey().Description, role);
        }
    }
}