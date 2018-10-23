using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Enums;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Repositories
{
    public class IdentityRepository<TIdentityDbContext, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        : IIdentityRepository<TIdentityDbContext, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
    {
        private readonly TIdentityDbContext _dbContext;
        private readonly UserManager<TUser> _userManager;
        private readonly RoleManager<TRole> _roleManager;
        private readonly IMapper _mapper;

        public bool AutoSaveChanges { get; set; } = true;

        public IdentityRepository(TIdentityDbContext dbContext,
            UserManager<TUser> userManager,
            RoleManager<TRole> roleManager,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public virtual TUserKey ConvertUserKeyFromString(string id)
        {
            if (id == null)
            {
                return default(TUserKey);
            }
            return (TUserKey)TypeDescriptor.GetConverter(typeof(TUserKey)).ConvertFromInvariantString(id);
        }

        public virtual TRoleKey ConvertRoleKeyFromString(string id)
        {
            if (id == null)
            {
                return default(TRoleKey);
            }
            return (TRoleKey)TypeDescriptor.GetConverter(typeof(TRoleKey)).ConvertFromInvariantString(id);
        }

        public Task<bool> ExistsUserAsync(string userId)
        {
            var id = ConvertUserKeyFromString(userId);

            return _userManager.Users.AnyAsync(x => x.Id.Equals(id));
        }

        public Task<bool> ExistsRoleAsync(string roleId)
        {
            var id = ConvertRoleKeyFromString(roleId);

            return _roleManager.Roles.AnyAsync(x => x.Id.Equals(id));
        }

        public async Task<PagedList<TUser>> GetUsersAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<TUser>();
            Expression<Func<TUser, bool>> searchCondition = x => x.UserName.Contains(search) || x.Email.Contains(search);

            var users = await _userManager.Users.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.Id, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(users);

            pagedList.TotalCount = await _userManager.Users.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public Task<List<TRole>> GetRolesAsync()
        {
            return _roleManager.Roles.ToListAsync();
        }

        public async Task<PagedList<TRole>> GetRolesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<TRole>();

            Expression<Func<TRole, bool>> searchCondition = x => x.Name.Contains(search);
            var roles = await _roleManager.Roles.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.Id, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(roles);
            pagedList.TotalCount = await _roleManager.Roles.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public Task<TRole> GetRoleAsync(TKey roleId)
        {
            return _roleManager.Roles.Where(x => x.Id.Equals(roleId)).SingleOrDefaultAsync();
        }

        public async Task<(IdentityResult identityResult, TKey roleId)> CreateRoleAsync(TRole role)
        {
            var identityResult = await _roleManager.CreateAsync(role);

            return (identityResult, role.Id);
        }

        public async Task<(IdentityResult identityResult, TKey roleId)> UpdateRoleAsync(TRole role)
        {
            var thisRole = await _roleManager.FindByIdAsync(role.Id.ToString());
            thisRole.Name = role.Name;
            var identityResult = await _roleManager.UpdateAsync(thisRole);

            return (identityResult, role.Id);
        }

        public async Task<IdentityResult> DeleteRoleAsync(TRole role)
        {
            var thisRole = await _roleManager.FindByIdAsync(role.Id.ToString());

            return await _roleManager.DeleteAsync(thisRole);
        }

        public Task<TUser> GetUserAsync(string userId)
        {
            return _userManager.FindByIdAsync(userId);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>This method returns identity result and new user id</returns>
        public async Task<(IdentityResult identityResult, TKey userId)> CreateUserAsync(TUser user)
        {
            var identityResult = await _userManager.CreateAsync(user);

            return (identityResult, user.Id);
        }

        public async Task<(IdentityResult identityResult, TKey userId)> UpdateUserAsync(TUser user)
        {
            var userIdentity = await _userManager.FindByIdAsync(user.Id.ToString());            
            _mapper.Map(user, userIdentity);
            var identityResult = await _userManager.UpdateAsync(userIdentity);

            return (identityResult, user.Id);
        }

        public async Task<IdentityResult> CreateUserRoleAsync(string userId, string roleId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var selectRole = await _roleManager.FindByIdAsync(roleId);

            return await _userManager.AddToRoleAsync(user, selectRole.Name);
        }

        public async Task<PagedList<TRole>> GetUserRolesAsync(string userId, int page = 1, int pageSize = 10)
        {
            var id = ConvertUserKeyFromString(userId);

            var pagedList = new PagedList<TRole>();
            var roles = from r in _dbContext.Set<TRole>()
                        join ur in _dbContext.Set<TUserRole>() on r.Id equals ur.RoleId
                        where ur.UserId.Equals(id)
                        select r;

            var userIdentityRoles = await roles.PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(userIdentityRoles);
            pagedList.TotalCount = await roles.CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public async Task<IdentityResult> DeleteUserRoleAsync(string userId, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            var user = await _userManager.FindByIdAsync(userId);

            return await _userManager.RemoveFromRoleAsync(user, role.Name);
        }

        public async Task<PagedList<TUserClaim>> GetUserClaimsAsync(string userId, int page, int pageSize)
        {
            var id = ConvertUserKeyFromString(userId);
            var pagedList = new PagedList<TUserClaim>();

            var claims = await _dbContext.Set<TUserClaim>().Where(x => x.UserId.Equals(id))
                .PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(claims);
            pagedList.TotalCount = await _dbContext.Set<TUserClaim>().Where(x => x.UserId.Equals(id)).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public async Task<PagedList<TRoleClaim>> GetRoleClaimsAsync(string roleId, int page = 1, int pageSize = 10)
        {
            var id = ConvertRoleKeyFromString(roleId);
            var pagedList = new PagedList<TRoleClaim>();
            var claims = await _dbContext.Set<TRoleClaim>().Where(x => x.RoleId.Equals(id))
                .PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(claims);
            pagedList.TotalCount = await _dbContext.Set<TRoleClaim>().Where(x => x.RoleId.Equals(id)).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public Task<TUserClaim> GetUserClaimAsync(string userId, int claimId)
        {
            var userIdConverted = ConvertUserKeyFromString(userId);

            return _dbContext.Set<TUserClaim>().Where(x => x.UserId.Equals(userIdConverted) && x.Id == claimId)
                .SingleOrDefaultAsync();
        }

        public Task<TRoleClaim> GetRoleClaimAsync(string roleId, int claimId)
        {
            var roleIdConverted = ConvertRoleKeyFromString(roleId);

            return _dbContext.Set<TRoleClaim>().Where(x => x.RoleId.Equals(roleIdConverted) && x.Id == claimId)
                .SingleOrDefaultAsync();
        }

        public async Task<IdentityResult> CreateUserClaimsAsync(TUserClaim claims)
        {
            var user = await _userManager.FindByIdAsync(claims.UserId.ToString());
            return await _userManager.AddClaimAsync(user, new Claim(claims.ClaimType, claims.ClaimValue));
        }

        public async Task<IdentityResult> CreateRoleClaimsAsync(TRoleClaim claims)
        {
            var role = await _roleManager.FindByIdAsync(claims.RoleId.ToString());
            return await _roleManager.AddClaimAsync(role, new Claim(claims.ClaimType, claims.ClaimValue));
        }

        public async Task<int> DeleteUserClaimsAsync(string userId, int claimId)
        {
            var userClaim = await _dbContext.Set<TUserClaim>().Where(x => x.Id == claimId).SingleOrDefaultAsync();

            _dbContext.UserClaims.Remove(userClaim);

            return await AutoSaveChangesAsync();
        }

        public async Task<int> DeleteRoleClaimsAsync(string roleId, int claimId)
        {
            var roleClaim = await _dbContext.Set<TRoleClaim>().Where(x => x.Id == claimId).SingleOrDefaultAsync();

            _dbContext.Set<TRoleClaim>().Remove(roleClaim);

            return await AutoSaveChangesAsync();
        }

        public async Task<List<UserLoginInfo>> GetUserProvidersAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userLoginInfos = await _userManager.GetLoginsAsync(user);

            return userLoginInfos.ToList();
        }

        public Task<TUserLogin> GetUserProviderAsync(string userId, string providerKey)
        {
            var userIdConverted = ConvertUserKeyFromString(userId);

            return _dbContext.Set<TUserLogin>().Where(x => x.UserId.Equals(userIdConverted) && x.ProviderKey == providerKey)
                .SingleOrDefaultAsync();
        }

        public async Task<IdentityResult> DeleteUserProvidersAsync(string userId, string providerKey, string loginProvider)
        {
            var userIdConverted = ConvertUserKeyFromString(userId);

            var user = await _userManager.FindByIdAsync(userId);
            var login = await _dbContext.Set<TUserLogin>().Where(x => x.UserId.Equals(userIdConverted) && x.ProviderKey == providerKey && x.LoginProvider == loginProvider).SingleOrDefaultAsync();
            return await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
        }

        public async Task<IdentityResult> UserChangePasswordAsync(string userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var userIdentity = await _userManager.FindByIdAsync(userId);

            return await _userManager.DeleteAsync(userIdentity);
        }

        private async Task<int> AutoSaveChangesAsync()
        {
            return AutoSaveChanges ? await _dbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
        }
    }
}