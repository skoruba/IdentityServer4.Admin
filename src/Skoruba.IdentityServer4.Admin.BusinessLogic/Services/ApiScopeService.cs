using System.Threading.Tasks;
using Skoruba.AuditLogging.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Events.ApiScope;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class ApiScopeService : IApiScopeService
    {
        protected readonly IApiScopeRepository ApiScopeRepository;
        protected readonly IApiScopeServiceResources ApiScopeServiceResources;
        protected readonly IAuditEventLogger AuditEventLogger;

        public ApiScopeService(IApiScopeServiceResources apiScopeServiceResources, IApiScopeRepository apiScopeRepository, IAuditEventLogger auditEventLogger)
        {
            ApiScopeRepository = apiScopeRepository;
            AuditEventLogger = auditEventLogger;
            ApiScopeServiceResources = apiScopeServiceResources;

        }

        public virtual async Task<ApiScopesDto> GetApiScopesAsync(int page = 1, int pageSize = 10)
        {
            var pagedList = await ApiScopeRepository.GetApiScopesAsync(page, pageSize);

            var apiScopesDto = pagedList.ToModel();

            await AuditEventLogger.LogEventAsync(new ApiScopesRequestedEvent(apiScopesDto));

            return apiScopesDto;
        }

        public virtual async Task<ApiScopesDto> GetApiScopeAsync(int apiScopeId)
        {
            var apiScope = await ApiScopeRepository.GetApiScopeAsync(apiScopeId);
            if (apiScope == null) throw new UserFriendlyErrorPageException(string.Format(ApiScopeServiceResources.ApiScopeDoesNotExist().Description, apiScopeId), ApiScopeServiceResources.ApiScopeDoesNotExist().Description);

            var apiScopesDto = apiScope.ToModel();

            await AuditEventLogger.LogEventAsync(new ApiScopeRequestedEvent(apiScopesDto));

            return apiScopesDto;
        }

        public virtual async Task<int> AddApiScopeAsync(ApiScopesDto apiScope)
        {
            var canInsert = await CanInsertApiScopeAsync(apiScope);
            if (!canInsert)
            {
                await BuildApiScopesViewModelAsync(apiScope);
                throw new UserFriendlyViewException(string.Format(ApiScopeServiceResources.ApiScopeExistsValue().Description, apiScope.Name), ApiScopeServiceResources.ApiScopeExistsKey().Description, apiScope);
            }

            var scope = apiScope.ToEntity();

            var added = await ApiScopeRepository.AddApiScopeAsync(scope);

            await AuditEventLogger.LogEventAsync(new ApiScopeAddedEvent(apiScope));

            return added;
        }

        public virtual ApiScopesDto BuildApiScopeViewModel(ApiScopesDto apiScope)
        {
            ComboBoxHelpers.PopulateValuesToList(apiScope.UserClaimsItems, apiScope.UserClaims);

            return apiScope;
        }

        private async Task BuildApiScopesViewModelAsync(ApiScopesDto apiScope)
        {
            if (apiScope.ApiScopeId == 0)
            {
                var apiScopesDto = await GetApiScopesAsync();
                apiScope.Scopes.AddRange(apiScopesDto.Scopes);
                apiScope.TotalCount = apiScopesDto.TotalCount;
            }
        }

        public virtual async Task<int> UpdateApiScopeAsync(ApiScopesDto apiScope)
        {
            var canInsert = await CanInsertApiScopeAsync(apiScope);
            if (!canInsert)
            {
                await BuildApiScopesViewModelAsync(apiScope);
                throw new UserFriendlyViewException(string.Format(ApiScopeServiceResources.ApiScopeExistsValue().Description, apiScope.Name), ApiScopeServiceResources.ApiScopeExistsKey().Description, apiScope);
            }

            var scope = apiScope.ToEntity();

            var originalApiScope = await GetApiScopeAsync(apiScope.ApiScopeId);

            var updated = await ApiScopeRepository.UpdateApiScopeAsync(scope);

            await AuditEventLogger.LogEventAsync(new ApiScopeUpdatedEvent(originalApiScope, apiScope));

            return updated;
        }

        public virtual async Task<int> DeleteApiScopeAsync(ApiScopesDto apiScope)
        {
            var scope = apiScope.ToEntity();

            var deleted = await ApiScopeRepository.DeleteApiScopeAsync(scope);

            await AuditEventLogger.LogEventAsync(new ApiScopeDeletedEvent(apiScope));

            return deleted;
        }

        public virtual async Task<bool> CanInsertApiScopeAsync(ApiScopesDto apiScopes)
        {
            var apiScope = apiScopes.ToEntity();

            return await ApiScopeRepository.CanInsertApiScopeAsync(apiScope);
        }
    }
}
