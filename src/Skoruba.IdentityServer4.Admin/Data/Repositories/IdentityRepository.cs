using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.Data.DbContexts;
using Skoruba.IdentityServer4.Admin.Data.Entities.Identity;
using Skoruba.IdentityServer4.Admin.Data.Mappers;
using Skoruba.IdentityServer4.Admin.Helpers;
using Skoruba.IdentityServer4.Admin.ViewModels.Common;
using Skoruba.IdentityServer4.Admin.ViewModels.Enums;

namespace Skoruba.IdentityServer4.Admin.Data.Repositories
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly AdminDbContext _dbContext;
        private readonly UserManager<UserIdentity> _userManager;
        private readonly RoleManager<UserIdentityRole> _roleManager;

        public bool AutoSaveChanges { get; set; } = true;

        public IdentityRepository(AdminDbContext dbContext,
            UserManager<UserIdentity> userManager,
            RoleManager<UserIdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> ExistsUserAsync(int userId)
        {
            var exists = await _dbContext.Users.AnyAsync(x => x.Id == userId);

            return exists;
        }

        public async Task<bool> ExistsRoleAsync(int roleId)
        {
            var exists = await _dbContext.Roles.AnyAsync(x => x.Id == roleId);

            return exists;
        }

        public async Task<PagedList<UserIdentity>> GetUsersAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<UserIdentity>();
            Expression<Func<UserIdentity, bool>> searchCondition = x => x.UserName.Contains(search) || x.Email.Contains(search);

            var users = await _dbContext.Users.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x=> x.Id, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(users);
            pagedList.TotalCount = await _dbContext.Users.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;
            
            return pagedList;
        }
        
        public async Task<List<UserIdentityRole>> GetRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            
            return roles;
        }

        public async Task<PagedList<UserIdentityRole>> GetRolesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<UserIdentityRole>();

            Expression<Func<UserIdentityRole, bool>> searchCondition = x=> x.Name.Contains(search);
            var roles = await _roleManager.Roles.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x=> x.Id, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(roles);
            pagedList.TotalCount = await _roleManager.Roles.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;
            
            return pagedList;
        }

        public async Task<UserIdentityRole> GetRoleAsync(UserIdentityRole role)
        {
            var existsRole = await _roleManager.Roles.Where(x => x.Id == role.Id).SingleOrDefaultAsync();

            return existsRole;
        }

        public async Task<IdentityResult> CreateRoleAsync(UserIdentityRole role)
        {
            var identityResult = await _roleManager.CreateAsync(role);

            return identityResult;
        }

        public async Task<IdentityResult> UpdateRoleAsync(UserIdentityRole role)
        {
            var thisRole = await _roleManager.FindByIdAsync(role.Id.ToString());
            thisRole.Name = role.Name;
            var identityResult = await _roleManager.UpdateAsync(thisRole);

            return identityResult;
        }

        public async Task<IdentityResult> DeleteRoleAsync(UserIdentityRole role)
        {
            var thisRole = await _roleManager.FindByIdAsync(role.Id.ToString());
            var identityResult = await _roleManager.DeleteAsync(thisRole);

            return identityResult;
        }

        public async Task<UserIdentity> GetUserAsync(UserIdentity user)
        {
            var userIdentity = await _userManager.FindByIdAsync(user.Id.ToString());
            
            return userIdentity;
        }

        public async Task<IdentityResult> CreateUserAsync(UserIdentity user)
        {
            var userIdentity = await _userManager.CreateAsync(user);

            return userIdentity;
        }

        public async Task<IdentityResult> UpdateUserAsync(UserIdentity user)
        {
            var userIdentity = await _userManager.FindByIdAsync(user.Id.ToString());
            if (userIdentity == null) return IdentityResult.Failed(new IdentityError() { Description = "User doesn't exists" });

            userIdentity.MapTo(user);

            var identityResult = await _userManager.UpdateAsync(userIdentity);

            return identityResult;
        }

        public async Task<IdentityResult> CreateUserRoleAsync(int userId, int roleId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var selectRole = await _roleManager.FindByIdAsync(roleId.ToString());

            var identityResult = await _userManager.AddToRoleAsync(user, selectRole.Name);

            return identityResult;
        }

        public async Task<PagedList<UserIdentityRole>> GetUserRolesAsync(int userId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<UserIdentityRole>();
            var roles = from r in _dbContext.Roles
                               join ur in _dbContext.UserRoles on r.Id equals ur.RoleId
                               where ur.UserId == userId
                              
                               select new UserIdentityRole { Id = r.Id, Name = r.Name };

            var userIdentityRoles = await roles.PageBy(x=> x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(userIdentityRoles);
            pagedList.TotalCount = await roles.CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public async Task<IdentityResult> DeleteUserRoleAsync(int userId, int roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            var user = await _userManager.FindByIdAsync(userId.ToString());

            var identityResult = await _userManager.RemoveFromRoleAsync(user, role.Name);

            return identityResult;
        }

        public async Task<PagedList<UserIdentityUserClaim>> GetUserClaimsAsync(int userId, int page, int pageSize)
        {
            var pagedList = new PagedList<UserIdentityUserClaim>();
            var claims = await _dbContext.UserClaims.Where(x => x.UserId == userId)
                .PageBy(x=> x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(claims);
            pagedList.TotalCount = await _dbContext.UserClaims.Where(x => x.UserId == userId).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public async Task<PagedList<UserIdentityRoleClaim>> GetRoleClaimsAsync(int roleId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<UserIdentityRoleClaim>();
            var claims = await _dbContext.RoleClaims.Where(x => x.RoleId == roleId)
                .PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(claims);
            pagedList.TotalCount = await _dbContext.RoleClaims.Where(x => x.RoleId == roleId).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public async Task<UserIdentityUserClaim> GetUserClaimAsync(int userId, int claimId)
        {
            var claim = await _dbContext.UserClaims.Where(x => x.UserId == userId && x.Id == claimId)                
                .SingleOrDefaultAsync();

            return claim;
        }

        public async Task<UserIdentityRoleClaim> GetRoleClaimAsync(int roleId, int claimId)
        {
            var claim = await _dbContext.RoleClaims.Where(x => x.RoleId == roleId && x.Id == claimId)                
                .SingleOrDefaultAsync();

            return claim;
        }

        public async Task<IdentityResult> CreateUserClaimsAsync(UserIdentityUserClaim claims)
        {
            var user = await _userManager.FindByIdAsync(claims.UserId.ToString());
            var identityResult = await _userManager.AddClaimAsync(user, new Claim(claims.ClaimType, claims.ClaimValue));

            return identityResult;
        }

        public async Task<IdentityResult> CreateRoleClaimsAsync(UserIdentityRoleClaim claims)
        {
            var role = await _roleManager.FindByIdAsync(claims.RoleId.ToString());
            var identityResult =
                await _roleManager.AddClaimAsync(role, new Claim(claims.ClaimType, claims.ClaimValue));

            return identityResult;
        }

        public async Task<int> DeleteUserClaimsAsync(int userId, int claimId)
        {
            var userClaim = await _dbContext.UserClaims.Where(x => x.Id == claimId).SingleOrDefaultAsync();

            _dbContext.UserClaims.Remove(userClaim);

            return await AutoSaveChangesAsync();
        }

        public async Task<int> DeleteRoleClaimsAsync(int roleId, int claimId)
        {
            var roleClaim = await _dbContext.RoleClaims.Where(x => x.Id == claimId).SingleOrDefaultAsync();

            _dbContext.RoleClaims.Remove(roleClaim);

            return await AutoSaveChangesAsync();
        }

        public async Task<List<UserLoginInfo>> GetUserProvidersAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var userLoginInfos = await _userManager.GetLoginsAsync(user);
            var userProviders = userLoginInfos.ToList();

            return userProviders;
        }

        public async Task<UserIdentityUserLogin> GetUserProviderAsync(int userId, string providerKey)
        {
            var login = await _dbContext.UserLogins.Where(x => x.UserId == userId && x.ProviderKey == providerKey)
                .SingleOrDefaultAsync();
            
            return login;
        }

        public async Task<IdentityResult> DeleteUserProvidersAsync(int userId, string providerKey, string loginProvider)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var login = await _dbContext.UserLogins.Where(x => x.UserId == userId && x.ProviderKey == providerKey && x.LoginProvider == loginProvider).SingleOrDefaultAsync();
            var identityResult = await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);

            return identityResult;
        }

        public async Task<IdentityResult> UserChangePasswordAsync(int userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var passwordChangedResult = await _userManager.ResetPasswordAsync(user, token, password);

            return passwordChangedResult;
        }

        public async Task<IdentityResult> DeleteUserAsync(UserIdentity user)
        {
            var userIdentity = await _userManager.FindByIdAsync(user.Id.ToString());

            var identityResult = await _userManager.DeleteAsync(userIdentity);

            return identityResult;
        }

        private async Task<int> AutoSaveChangesAsync()
        {
            return AutoSaveChanges ? await _dbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
        }
    }
}