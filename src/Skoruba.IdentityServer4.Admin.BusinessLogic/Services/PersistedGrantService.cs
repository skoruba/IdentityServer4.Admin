using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Grant;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class PersistedGrantService<TDbContext> : IPersistedGrantService<TDbContext>
        where TDbContext : DbContext, IAdminPersistedGrantDbContext
    {
        private readonly IPersistedGrantRepository<TDbContext> _persistedGrantRepository;
        private readonly IPersistedGrantServiceResources _persistedGrantServiceResources;

        public PersistedGrantService(IPersistedGrantRepository<TDbContext> persistedGrantRepository,
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
