using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skoruba.IdentityServer4.Admin.Api.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.Api.Dtos.ApiResources;
using Skoruba.IdentityServer4.Admin.Api.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.Api.Mappers;
using Skoruba.IdentityServer4.Admin.Api.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;

namespace Skoruba.IdentityServer4.Admin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json", "application/problem+json")]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public class ApiResourcesController : ControllerBase
    {
        private readonly IApiResourceService _apiResourceService;
        private readonly IApiErrorResources _errorResources;

        public ApiResourcesController(IApiResourceService apiResourceService, IApiErrorResources errorResources)
        {
            _apiResourceService = apiResourceService;
            _errorResources = errorResources;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResourcesApiDto>> Get(string searchText, int page = 1, int pageSize = 10)
        {
            var apiResourcesDto = await _apiResourceService.GetApiResourcesAsync(searchText, page, pageSize);
            var apiResourcesApiDto = apiResourcesDto.ToApiResourceApiModel<ApiResourcesApiDto>();

            return Ok(apiResourcesApiDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResourceApiDto>> Get(int id)
        {
            var apiResourceDto = await _apiResourceService.GetApiResourceAsync(id);
            var apiResourceApiDto = apiResourceDto.ToApiResourceApiModel<ApiResourceApiDto>();

            return Ok(apiResourceApiDto);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody]ApiResourceApiDto apiResourceApi)
        {
            var apiResourceDto = apiResourceApi.ToApiResourceApiModel<ApiResourceDto>();

            if (!apiResourceDto.Id.Equals(default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            var apiResourceId = await _apiResourceService.AddApiResourceAsync(apiResourceDto);
            apiResourceApi.Id = apiResourceId;

            return CreatedAtAction(nameof(Get), new { id = apiResourceId }, apiResourceApi);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]ApiResourceApiDto apiResourceApi)
        {
            var apiResourceDto = apiResourceApi.ToApiResourceApiModel<ApiResourceDto>();

            await _apiResourceService.GetApiResourceAsync(apiResourceDto.Id);
            await _apiResourceService.UpdateApiResourceAsync(apiResourceDto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var apiResourceDto = new ApiResourceDto { Id = id };

            await _apiResourceService.GetApiResourceAsync(apiResourceDto.Id);
            await _apiResourceService.DeleteApiResourceAsync(apiResourceDto);

            return Ok();
        }

        [HttpGet("{id}/Scopes")]
        public async Task<ActionResult<ApiScopesApiDto>> GetScopes(int id, int page = 1, int pageSize = 10)
        {
            var apiScopesDto = await _apiResourceService.GetApiScopesAsync(id, page, pageSize);
            var apiScopesApiDto = apiScopesDto.ToApiResourceApiModel<ApiScopesApiDto>();

            return Ok(apiScopesApiDto);
        }

        [HttpGet("{id}/Scopes/{scopeId}")]
        public async Task<ActionResult<ApiScopeApiDto>> GetScope(int id, int scopeId)
        {
            var apiScopesDto = await _apiResourceService.GetApiScopeAsync(id, scopeId);
            var apiScopeApiDto = apiScopesDto.ToApiResourceApiModel<ApiScopeApiDto>();

            return Ok(apiScopeApiDto);
        }

        [HttpPost("{id}/Scopes")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostScope(int id, [FromBody]ApiScopeApiDto apiScopeApi)
        {
            var apiScope = apiScopeApi.ToApiResourceApiModel<ApiScopesDto>();
            apiScope.ApiResourceId = id;

            if (!apiScope.ApiScopeId.Equals(default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            await _apiResourceService.GetApiResourceAsync(apiScope.ApiResourceId);
            var scopeId = await _apiResourceService.AddApiScopeAsync(apiScope);
            apiScope.ApiScopeId = scopeId;

            return CreatedAtAction(nameof(GetScope), new { id, scopeId }, apiScope);
        }

        [HttpPut("{id}/Scopes")]
        public async Task<IActionResult> PutScope(int id, [FromBody]ApiScopeApiDto apiScopeApi)
        {
            var apiScope = apiScopeApi.ToApiResourceApiModel<ApiScopesDto>();
            apiScope.ApiResourceId = id;

            await _apiResourceService.GetApiResourceAsync(apiScope.ApiResourceId);
            await _apiResourceService.GetApiScopeAsync(apiScope.ApiResourceId, apiScope.ApiScopeId);

            await _apiResourceService.UpdateApiScopeAsync(apiScope);

            return Ok();
        }

        [HttpDelete("{id}/Scopes/{apiScopeId}")]
        public async Task<IActionResult> DeleteScope(int id, int apiScopeId)
        {
            var apiScope = new ApiScopesDto { ApiResourceId = id, ApiScopeId = apiScopeId };

            await _apiResourceService.GetApiResourceAsync(apiScope.ApiResourceId);
            await _apiResourceService.GetApiScopeAsync(apiScope.ApiResourceId, apiScope.ApiScopeId);

            await _apiResourceService.DeleteApiScopeAsync(apiScope);

            return Ok();
        }

        [HttpGet("{id}/Secrets")]
        public async Task<ActionResult<ApiSecretsApiDto>> GetSecrets(int id, int page = 1, int pageSize = 10)
        {
            var apiSecretsDto = await _apiResourceService.GetApiSecretsAsync(id, page, pageSize);
            var apiSecretsApiDto = apiSecretsDto.ToApiResourceApiModel<ApiSecretsApiDto>();

            return Ok(apiSecretsApiDto);
        }

        [HttpGet("Secrets/{secretId}")]
        public async Task<ActionResult<ApiSecretApiDto>> GetSecret(int secretId)
        {
            var apiSecretsDto = await _apiResourceService.GetApiSecretAsync(secretId);
            var apiSecretApiDto = apiSecretsDto.ToApiResourceApiModel<ApiSecretApiDto>();

            return Ok(apiSecretApiDto);
        }

        [HttpPost("{id}/Secrets")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostSecret(int id, [FromBody]ApiSecretApiDto clientSecretApi)
        {
            var secretsDto = clientSecretApi.ToApiResourceApiModel<ApiSecretsDto>();
            secretsDto.ApiResourceId = id;

            if (!secretsDto.ApiSecretId.Equals(default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            var secretId = await _apiResourceService.AddApiSecretAsync(secretsDto);
            clientSecretApi.Id = secretId;

            return CreatedAtAction(nameof(GetSecret), new { secretId }, clientSecretApi);
        }

        [HttpDelete("Secrets/{secretId}")]
        public async Task<IActionResult> DeleteSecret(int secretId)
        {
            var apiSecret = new ApiSecretsDto { ApiSecretId = secretId };

            await _apiResourceService.GetApiSecretAsync(apiSecret.ApiSecretId);
            await _apiResourceService.DeleteApiSecretAsync(apiSecret);

            return Ok();
        }

        [HttpGet("{id}/Properties")]
        public async Task<ActionResult<ApiResourcePropertiesApiDto>> GetProperties(int id, int page = 1, int pageSize = 10)
        {
            var apiResourcePropertiesDto = await _apiResourceService.GetApiResourcePropertiesAsync(id, page, pageSize);
            var apiResourcePropertiesApiDto = apiResourcePropertiesDto.ToApiResourceApiModel<ApiResourcePropertiesApiDto>();

            return Ok(apiResourcePropertiesApiDto);
        }

        [HttpGet("Properties/{propertyId}")]
        public async Task<ActionResult<ApiResourcePropertyApiDto>> GetProperty(int propertyId)
        {
            var apiResourcePropertiesDto = await _apiResourceService.GetApiResourcePropertyAsync(propertyId);
            var apiResourcePropertyApiDto = apiResourcePropertiesDto.ToApiResourceApiModel<ApiResourcePropertyApiDto>();

            return Ok(apiResourcePropertyApiDto);
        }

        [HttpPost("{id}/Properties")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostProperty(int id, [FromBody]ApiResourcePropertyApiDto apiPropertyApi)
        {
            var apiResourcePropertiesDto = apiPropertyApi.ToApiResourceApiModel<ApiResourcePropertiesDto>();
            apiResourcePropertiesDto.ApiResourceId = id;

            if (!apiResourcePropertiesDto.ApiResourcePropertyId.Equals(default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            var propertyId = await _apiResourceService.AddApiResourcePropertyAsync(apiResourcePropertiesDto);
            apiPropertyApi.Id = propertyId;

            return CreatedAtAction(nameof(GetProperty), new { propertyId }, apiPropertyApi);
        }

        [HttpDelete("Properties/{propertyId}")]
        public async Task<IActionResult> DeleteProperty(int propertyId)
        {
            var apiResourceProperty = new ApiResourcePropertiesDto { ApiResourcePropertyId = propertyId };

            await _apiResourceService.GetApiResourcePropertyAsync(apiResourceProperty.ApiResourcePropertyId);
            await _apiResourceService.DeleteApiResourcePropertyAsync(apiResourceProperty);

            return Ok();
        }
    }
}