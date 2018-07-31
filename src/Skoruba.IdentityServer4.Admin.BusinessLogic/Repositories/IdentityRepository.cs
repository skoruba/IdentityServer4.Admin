using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Enums;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories
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

        public Task<bool> ExistsUserAsync(int userId)
        {
            return _dbContext.Users.AnyAsync(x => x.Id == userId);
        }

        public Task<bool> ExistsRoleAsync(int roleId)
        {
            return _dbContext.Roles.AnyAsync(x => x.Id == roleId);
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
        
        public Task<List<UserIdentityRole>> GetRolesAsync()
        {
            return _roleManager.Roles.ToListAsync();
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

        public Task<UserIdentityRole> GetRoleAsync(UserIdentityRole role)
        {
            return _roleManager.Roles.Where(x => x.Id == role.Id).SingleOrDefaultAsync();            
        }

        public Task<IdentityResult> CreateRoleAsync(UserIdentityRole role)
        {
            return _roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult> UpdateRoleAsync(UserIdentityRole role)
        {
            var thisRole = await _roleManager.FindByIdAsync(role.Id.ToString());
            thisRole.Name = role.Name;

            return await _roleManager.UpdateAsync(thisRole);            
        }

        public async Task<IdentityResult> DeleteRoleAsync(UserIdentityRole role)
        {
            var thisRole = await _roleManager.FindByIdAsync(role.Id.ToString());

            return await _roleManager.DeleteAsync(thisRole);
        }

        public Task<UserIdentity> GetUserAsync(UserIdentity user)
        {
            return _userManager.FindByIdAsync(user.Id.ToString());
        }

        public Task<IdentityResult> CreateUserAsync(UserIdentity user)
        {
            return _userManager.CreateAsync(user);            
        }

        public async Task<IdentityResult> UpdateUserAsync(UserIdentity user)
        {
            var userIdentity = await _userManager.FindByIdAsync(user.Id.ToString());
            if (userIdentity == null) return IdentityResult.Failed(new IdentityError() { Description = "User doesn't exists" });

            userIdentity.MapTo(user);

            return await _userManager.UpdateAsync(userIdentity);            
        }

        public async Task<IdentityResult> CreateUserRoleAsync(int userId, int roleId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var selectRole = await _roleManager.FindByIdAsync(roleId.ToString());

            return await _userManager.AddToRoleAsync(user, selectRole.Name);            
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

            return await _userManager.RemoveFromRoleAsync(user, role.Name);
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

        public Task<UserIdentityUserClaim> GetUserClaimAsync(int userId, int claimId)
        {
            return _dbContext.UserClaims.Where(x => x.UserId == userId && x.Id == claimId)                
                .SingleOrDefaultAsync();
        }

        public Task<UserIdentityRoleClaim> GetRoleClaimAsync(int roleId, int claimId)
        {
            return _dbContext.RoleClaims.Where(x => x.RoleId == roleId && x.Id == claimId)                
                .SingleOrDefaultAsync();
        }

        public async Task<IdentityResult> CreateUserClaimsAsync(UserIdentityUserClaim claims)
        {
            var user = await _userManager.FindByIdAsync(claims.UserId.ToString());
            return await _userManager.AddClaimAsync(user, new Claim(claims.ClaimType, claims.ClaimValue));
        }

        public async Task<IdentityResult> CreateRoleClaimsAsync(UserIdentityRoleClaim claims)
        {
            var role = await _roleManager.FindByIdAsync(claims.RoleId.ToString());
            return await _roleManager.AddClaimAsync(role, new Claim(claims.ClaimType, claims.ClaimValue));
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

            return userLoginInfos.ToList();
        }

        public Task<UserIdentityUserLogin> GetUserProviderAsync(int userId, string providerKey)
        {
            return _dbContext.UserLogins.Where(x => x.UserId == userId && x.ProviderKey == providerKey)
                .SingleOrDefaultAsync();
        }

        public async Task<IdentityResult> DeleteUserProvidersAsync(int userId, string providerKey, string loginProvider)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var login = await _dbContext.UserLogins.Where(x => x.UserId == userId && x.ProviderKey == providerKey && x.LoginProvider == loginProvider).SingleOrDefaultAsync();
            return await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);            
        }

        public async Task<IdentityResult> UserChangePasswordAsync(int userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<IdentityResult> DeleteUserAsync(UserIdentity user)
        {
            var userIdentity = await _userManager.FindByIdAsync(user.Id.ToString());

            return await _userManager.DeleteAsync(userIdentity);
        }

        private async Task<int> AutoSaveChangesAsync()
        {
            return AutoSaveChanges ? await _dbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
        }
    }
}