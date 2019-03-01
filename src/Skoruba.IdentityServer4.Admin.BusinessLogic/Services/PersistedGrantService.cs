using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Grant;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class PersistedGrantService : IPersistedGrantService
    {
        protected readonly IPersistedGrantRepository PersistedGrantRepository;
        protected readonly IPersistedGrantServiceResources PersistedGrantServiceResources;

        public PersistedGrantService(IPersistedGrantRepository persistedGrantRepository,
            IPersistedGrantServiceResources persistedGrantServiceResources)
        {
            PersistedGrantRepository = persistedGrantRepository;
            PersistedGrantServiceResources = persistedGrantServiceResources;
        }

        public virtual async Task<PersistedGrantsDto> GetPersistedGrantsByUsers(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await PersistedGrantRepository.GetPersistedGrantsByUsers(search, page, pageSize);
            var persistedGrantsDto = pagedList.ToModel();

            return persistedGrantsDto;
        }

        public virtual async Task<PersistedGrantsDto> GetPersistedGrantsByUser(string subjectId, int page = 1, int pageSize = 10)
        {
            var exists = await PersistedGrantRepository.ExistsPersistedGrantsAsync(subjectId);
            if (!exists) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description, subjectId), PersistedGrantServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description);

            var pagedList = await PersistedGrantRepository.GetPersistedGrantsByUser(subjectId, page, pageSize);
            var persistedGrantsDto = pagedList.ToModel();

            return persistedGrantsDto;
        }

        public virtual async Task<PersistedGrantDto> GetPersistedGrantAsync(string key)
        {
            var persistedGrant = await PersistedGrantRepository.GetPersistedGrantAsync(key);
            if (persistedGrant == null) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantServiceResources.PersistedGrantDoesNotExist().Description, key), PersistedGrantServiceResources.PersistedGrantDoesNotExist().Description);
            var persistedGrantDto = persistedGrant.ToModel();

            return persistedGrantDto;
        }

        public virtual async Task<int> DeletePersistedGrantAsync(string key)
        {
            return await PersistedGrantRepository.DeletePersistedGrantAsync(key);
        }

        public virtual async Task<int> DeletePersistedGrantsAsync(string userId)
        {
            return await PersistedGrantRepository.DeletePersistedGrantsAsync(userId);
        }
    }
}
