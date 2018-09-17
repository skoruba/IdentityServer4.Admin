using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Grant;
using Skoruba.IdentityServer4.Admin.BusinessLogic.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class PersistedGrantService<TDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IPersistedGrantService<TDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IAdminPersistedGrantIdentityDbContext
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
    {
        private readonly IPersistedGrantRepository<TDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> _persistedGrantRepository;
        private readonly IPersistedGrantServiceResources _persistedGrantServiceResources;

        public PersistedGrantService(IPersistedGrantRepository<TDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> persistedGrantRepository,
            IPersistedGrantServiceResources persistedGrantServiceResources)
        {
            _persistedGrantRepository = persistedGrantRepository;
            _persistedGrantServiceResources = persistedGrantServiceResources;
        }

        public async Task<PersistedGrantsDto> GetPersitedGrantsByUsers(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await _persistedGrantRepository.GetPersitedGrantsByUsers(search, page, pageSize);
            var persistedGrantsDto = pagedList.ToModel();

            return persistedGrantsDto;
        }

        public async Task<PersistedGrantsDto> GetPersitedGrantsByUser(string subjectId, int page = 1, int pageSize = 10)
        {
            var exists = await _persistedGrantRepository.ExistsPersistedGrantsAsync(subjectId);
            if (!exists) throw new UserFriendlyErrorPageException(string.Format(_persistedGrantServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description, subjectId), _persistedGrantServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description);

            var pagedList = await _persistedGrantRepository.GetPersitedGrantsByUser(subjectId, page, pageSize);
            var persistedGrantsDto = pagedList.ToModel();

            return persistedGrantsDto;
        }

        public async Task<PersistedGrantDto> GetPersitedGrantAsync(string key)
        {
            var persistedGrant = await _persistedGrantRepository.GetPersitedGrantAsync(key);
            if (persistedGrant == null) throw new UserFriendlyErrorPageException(string.Format(_persistedGrantServiceResources.PersistedGrantDoesNotExist().Description, key), _persistedGrantServiceResources.PersistedGrantDoesNotExist().Description);
            var persistedGrantDto = persistedGrant.ToModel();

            return persistedGrantDto;
        }

        public async Task<int> DeletePersistedGrantAsync(string key)
        {
            return await _persistedGrantRepository.DeletePersistedGrantAsync(key);
        }

        public async Task<int> DeletePersistedGrantsAsync(string userId)
        {
            return await _persistedGrantRepository.DeletePersistedGrantsAsync(userId);
        }
    }
}
