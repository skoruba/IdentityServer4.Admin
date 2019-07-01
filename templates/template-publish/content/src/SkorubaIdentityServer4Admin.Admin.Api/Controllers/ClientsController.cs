using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkorubaIdentityServer4Admin.Admin.Api.Configuration.Constants;
using SkorubaIdentityServer4Admin.Admin.Api.Dtos.Clients;
using SkorubaIdentityServer4Admin.Admin.Api.ExceptionHandling;
using SkorubaIdentityServer4Admin.Admin.Api.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;

namespace SkorubaIdentityServer4Admin.Admin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Policy = AuthorizationConsts.AdministrationPolicy)]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<ActionResult<ClientsApiDto>> Get(string searchText, int page = 1, int pageSize = 10)
        {
            var clientsDto = await _clientService.GetClientsAsync(searchText, page, pageSize);
            var clientsApiDto = clientsDto.ToClientApiModel<ClientsApiDto>();

            return Ok(clientsApiDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientApiDto>> Get(int id)
        {
            var clientDto = await _clientService.GetClientAsync(id);
            var clientApiDto = clientDto.ToClientApiModel<ClientApiDto>();

            return Ok(clientApiDto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ClientApiDto client)
        {
            var clientDto = client.ToClientApiModel<ClientDto>();
            await _clientService.AddClientAsync(clientDto);

            return Ok();
        }
        
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]ClientApiDto client)
        {
            var clientDto = client.ToClientApiModel<ClientDto>();

            await _clientService.GetClientAsync(clientDto.Id);
            await _clientService.UpdateClientAsync(clientDto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var clientDto = new ClientDto { Id = id };

            await _clientService.GetClientAsync(clientDto.Id);
            await _clientService.RemoveClientAsync(clientDto);

            return Ok();
        }

        [HttpPost("Clone")]
        public async Task<IActionResult> PostClientClone([FromBody]ClientCloneApiDto client)
        {
            var clientCloneDto = client.ToClientApiModel<ClientCloneDto>();

            await _clientService.GetClientAsync(clientCloneDto.Id);
            await _clientService.CloneClientAsync(clientCloneDto);

            return Ok();
        }

        [HttpGet("{id}/Secrets")]
        public async Task<ActionResult<ClientSecretsApiDto>> GetSecrets(int id, int page = 1, int pageSize = 10)
        {
            var clientSecretsDto = await _clientService.GetClientSecretsAsync(id, page, pageSize);
            var clientSecretsApiDto = clientSecretsDto.ToClientApiModel<ClientSecretsApiDto>();

            return Ok(clientSecretsApiDto);
        }

        [HttpGet("Secrets/{secretId}")]
        public async Task<ActionResult<ClientSecretApiDto>> GetSecret(int secretId)
        {
            var clientSecretsDto = await _clientService.GetClientSecretAsync(secretId);
            var clientSecretDto = clientSecretsDto.ToClientApiModel<ClientSecretApiDto>();

            return Ok(clientSecretDto);
        }

        [HttpPost("{id}/Secrets")]
        public async Task<IActionResult> PostSecret(int id, [FromBody]ClientSecretApiDto clientSecretApi)
        {
            var secretsDto = clientSecretApi.ToClientApiModel<ClientSecretsDto>();
            secretsDto.ClientId = id;

            await _clientService.AddClientSecretAsync(secretsDto);

            return Ok();
        }

        [HttpDelete("Secrets/{secretId}")]
        public async Task<IActionResult> DeleteSecret(int secretId)
        {
            var clientSecret = new ClientSecretsDto { ClientSecretId = secretId };

            await _clientService.GetClientSecretAsync(clientSecret.ClientSecretId);
            await _clientService.DeleteClientSecretAsync(clientSecret);

            return Ok();
        }

        [HttpGet("{id}/Properties")]
        public async Task<ActionResult<ClientPropertiesApiDto>> GetProperties(int id, int page = 1, int pageSize = 10)
        {
            var clientPropertiesDto = await _clientService.GetClientPropertiesAsync(id, page, pageSize);
            var clientPropertiesApiDto = clientPropertiesDto.ToClientApiModel<ClientPropertiesApiDto>();

            return Ok(clientPropertiesApiDto);
        }

        [HttpGet("Properties/{propertyId}")]
        public async Task<ActionResult<ClientPropertyApiDto>> GetProperty(int propertyId)
        {
            var clientPropertiesDto = await _clientService.GetClientPropertyAsync(propertyId);
            var clientPropertyApiDto = clientPropertiesDto.ToClientApiModel<ClientPropertyApiDto>();

            return Ok(clientPropertyApiDto);
        }

        [HttpPost("{id}/Properties")]
        public async Task<IActionResult> PostProperty(int id, [FromBody]ClientPropertyApiDto clientPropertyApi)
        {
            var clientPropertiesDto = clientPropertyApi.ToClientApiModel<ClientPropertiesDto>();
            clientPropertiesDto.ClientId = id;

            await _clientService.AddClientPropertyAsync(clientPropertiesDto);

            return Ok();
        }

        [HttpDelete("Properties/{propertyId}")]
        public async Task<IActionResult> DeleteProperty(int propertyId)
        {
            var clientProperty = new ClientPropertiesDto { ClientPropertyId = propertyId };

            await _clientService.GetClientPropertyAsync(clientProperty.ClientPropertyId);
            await _clientService.DeleteClientPropertyAsync(clientProperty);

            return Ok();
        }

        [HttpGet("{id}/Claims")]
        public async Task<ActionResult<ClientClaimsApiDto>> GetClaims(int id, int page = 1, int pageSize = 10)
        {
            var clientClaimsDto = await _clientService.GetClientClaimsAsync(id, page, pageSize);
            var clientClaimsApiDto = clientClaimsDto.ToClientApiModel<ClientClaimsApiDto>();

            return Ok(clientClaimsApiDto);
        }

        [HttpGet("Claims/{claimId}")]
        public async Task<ActionResult<ClientClaimApiDto>> GetClaim(int claimId)
        {
            var clientClaimsDto = await _clientService.GetClientClaimAsync(claimId);
            var clientClaimApiDto = clientClaimsDto.ToClientApiModel<ClientClaimApiDto>();

            return Ok(clientClaimApiDto);
        }

        [HttpPost("{id}/Claims")]
        public async Task<IActionResult> PostClaim(int id, [FromBody]ClientClaimApiDto clientClaimApiDto)
        {
            var clientClaimsDto = clientClaimApiDto.ToClientApiModel<ClientClaimsDto>();
            clientClaimsDto.ClientId = id;

            await _clientService.AddClientClaimAsync(clientClaimsDto);

            return Ok();
        }

        [HttpDelete("Claims/{claimId}")]
        public async Task<IActionResult> DeleteClaim(int claimId)
        {
            var clientClaimsDto = new ClientClaimsDto { ClientClaimId = claimId };

            await _clientService.GetClientClaimAsync(claimId);
            await _clientService.DeleteClientClaimAsync(clientClaimsDto);

            return Ok();
        }
    }
}