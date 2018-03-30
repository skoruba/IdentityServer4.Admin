using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Skoruba.IdentityServer4.Admin.Data.Mappers;
using Skoruba.IdentityServer4.Admin.Data.Repositories;
using Skoruba.IdentityServer4.Admin.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.ViewModels.Grant;

namespace Skoruba.IdentityServer4.Admin.Services
{
    public class PersistedGrantService : IPersistedGrantService
    {
        private readonly IPersistedGrantRepository _persistedGrantRepository;
        private readonly IStringLocalizer<PersistedGrantService> _localizer;
        
        public PersistedGrantService(IPersistedGrantRepository persistedGrantRepository,
            IStringLocalizer<PersistedGrantService> localizer)
        {
            _persistedGrantRepository = persistedGrantRepository;
            _localizer = localizer;
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
            if(!exists) throw new UserFriendlyErrorPageException(string.Format(_localizer["PersistedGrantWithSubjectIdDoesNotExist"], subjectId), _localizer["PersistedGrantWithSubjectIdDoesNotExist"]);

            var pagedList = await _persistedGrantRepository.GetPersitedGrantsByUser(subjectId, page, pageSize);
            var persistedGrantsDto = pagedList.ToModel();

            return persistedGrantsDto;
        }

        public async Task<PersistedGrantDto> GetPersitedGrantAsync(string key)
        {
            var persistedGrant = await _persistedGrantRepository.GetPersitedGrantAsync(key);
            if(persistedGrant == null) throw new UserFriendlyErrorPageException(string.Format(_localizer["PersistedGrantDoesNotExist"], key), _localizer["PersistedGrantDoesNotExist"]);
            var persistedGrantDto = persistedGrant.ToModel();

            return persistedGrantDto;
        }

        public async Task<int> DeletePersistedGrantAsync(string key)
        {
            var saved = await _persistedGrantRepository.DeletePersistedGrantAsync(key);

            return saved;
        }

        public async Task<int> DeletePersistedGrantsAsync(int userId)
        {
            var saved = await _persistedGrantRepository.DeletePersistedGrantsAsync(userId);

            return saved;
        }
    }
}
