using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.ExceptionHandling;

namespace Skoruba.IdentityServer4.Admin.Controllers
{
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    public class ConfigurationController : BaseController
    {
        private readonly IIdentityResourceService _identityResourceService;
        private readonly IApiResourceService _apiResourceService;
        private readonly IClientService _clientService;
        private readonly IStringLocalizer<ConfigurationController> _localizer;

        public ConfigurationController(IIdentityResourceService identityResourceService,
            IApiResourceService apiResourceService,
            IClientService clientService,
            IStringLocalizer<ConfigurationController> localizer,
            ILogger<ConfigurationController> logger)
            : base(logger)
        {
            _identityResourceService = identityResourceService;
            _apiResourceService = apiResourceService;
            _clientService = clientService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        [Route("[controller]/[action]/{id:int}")]
        public async Task<IActionResult> Client(int id)
        {
            if (id == 0)
            {
                var clientDto = _clientService.BuildClientViewModel();
                return View(clientDto);
            }

            var client = await _clientService.GetClientAsync((int)id);
            client = _clientService.BuildClientViewModel(client);

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Client(ClientDto client)
        {
            client = _clientService.BuildClientViewModel(client);

            if (!ModelState.IsValid)
            {
                return View(client);
            }

            //Add new client
            if (client.Id == 0)
            {
                var clientId = await _clientService.AddClientAsync(client);
                SuccessNotification(string.Format(_localizer["SuccessAddClient"], client.ClientId), _localizer["SuccessTitle"]);

                return RedirectToAction(nameof(Client), new { Id = clientId });
            }

            //Update client
            await _clientService.UpdateClientAsync(client);
            SuccessNotification(string.Format(_localizer["SuccessUpdateClient"], client.ClientId), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(Client), new { client.Id });
        }

        [HttpGet]
        public async Task<IActionResult> ClientClone(int id)
        {
            if (id == 0) return NotFound();

            var clientDto = await _clientService.GetClientAsync(id);
            var client = _clientService.BuildClientCloneViewModel(id, clientDto);

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClientClone(ClientCloneDto client)
        {
            if (!ModelState.IsValid)
            {
                return View(client);
            }

            var newClientId = await _clientService.CloneClientAsync(client);
            SuccessNotification(string.Format(_localizer["SuccessClientClone"], client.ClientId), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(Client), new { Id = newClientId });
        }

        [HttpGet]
        public async Task<IActionResult> ClientDelete(int id)
        {
            if (id == 0) return NotFound();

            var client = await _clientService.GetClientAsync(id);

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClientDelete(ClientDto client)
        {
            await _clientService.RemoveClientAsync(client);

            SuccessNotification(_localizer["SuccessClientDelete"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(Clients));
        }

        [HttpGet]
        public async Task<IActionResult> ClientClaims(int id, int? page)
        {
            if (id == 0) return NotFound();

            var claims = await _clientService.GetClientClaimsAsync(id, page ?? 1);

            return View(claims);
        }

        [HttpGet]
        public async Task<IActionResult> ClientProperties(int id, int? page)
        {
            if (id == 0) return NotFound();

            var properties = await _clientService.GetClientPropertiesAsync(id, page ?? 1);

            return View(properties);
        }

        [HttpGet]
        public async Task<IActionResult> ApiResourceProperties(int id, int? page)
        {
            if (id == 0) return NotFound();

            var properties = await _apiResourceService.GetApiResourcePropertiesAsync(id, page ?? 1);

            return View(properties);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApiResourceProperties(ApiResourcePropertiesDto apiResourceProperty)
        {
            if (!ModelState.IsValid)
            {
                return View(apiResourceProperty);
            }

            await _apiResourceService.AddApiResourcePropertyAsync(apiResourceProperty);
            SuccessNotification(string.Format(_localizer["SuccessAddApiResourceProperty"], apiResourceProperty.Key, apiResourceProperty.ApiResourceName), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(ApiResourceProperties), new { Id = apiResourceProperty.ApiResourceId });
        }

        [HttpGet]
        public async Task<IActionResult> IdentityResourceProperties(int id, int? page)
        {
            if (id == 0) return NotFound();

            var properties = await _identityResourceService.GetIdentityResourcePropertiesAsync(id, page ?? 1);

            return View(properties);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IdentityResourceProperties(IdentityResourcePropertiesDto identityResourceProperty)
        {
            if (!ModelState.IsValid)
            {
                return View(identityResourceProperty);
            }

            await _identityResourceService.AddIdentityResourcePropertyAsync(identityResourceProperty);
            SuccessNotification(string.Format(_localizer["SuccessAddIdentityResourceProperty"], identityResourceProperty.Value, identityResourceProperty.IdentityResourceName), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(IdentityResourceProperties), new { Id = identityResourceProperty.IdentityResourceId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClientProperties(ClientPropertiesDto clientProperty)
        {
            if (!ModelState.IsValid)
            {
                return View(clientProperty);
            }

            await _clientService.AddClientPropertyAsync(clientProperty);
            SuccessNotification(string.Format(_localizer["SuccessAddClientProperty"], clientProperty.ClientId, clientProperty.ClientName), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(ClientProperties), new { Id = clientProperty.ClientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClientClaims(ClientClaimsDto clientClaim)
        {
            if (!ModelState.IsValid)
            {
                return View(clientClaim);
            }

            await _clientService.AddClientClaimAsync(clientClaim);
            SuccessNotification(string.Format(_localizer["SuccessAddClientClaim"], clientClaim.Value, clientClaim.ClientName), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(ClientClaims), new { Id = clientClaim.ClientId });
        }

        [HttpGet]
        public async Task<IActionResult> ClientClaimDelete(int id)
        {
            if (id == 0) return NotFound();

            var clientClaim = await _clientService.GetClientClaimAsync(id);

            return View(nameof(ClientClaimDelete), clientClaim);
        }

        [HttpGet]
        public async Task<IActionResult> ClientPropertyDelete(int id)
        {
            if (id == 0) return NotFound();

            var clientProperty = await _clientService.GetClientPropertyAsync(id);

            return View(nameof(ClientPropertyDelete), clientProperty);
        }

        [HttpGet]
        public async Task<IActionResult> ApiResourcePropertyDelete(int id)
        {
            if (id == 0) return NotFound();

            var apiResourceProperty = await _apiResourceService.GetApiResourcePropertyAsync(id);

            return View(nameof(ApiResourcePropertyDelete), apiResourceProperty);
        }

        [HttpGet]
        public async Task<IActionResult> IdentityResourcePropertyDelete(int id)
        {
            if (id == 0) return NotFound();

            var identityResourceProperty = await _identityResourceService.GetIdentityResourcePropertyAsync(id);

            return View(nameof(IdentityResourcePropertyDelete), identityResourceProperty);
        }

        [HttpPost]
        public async Task<IActionResult> ClientClaimDelete(ClientClaimsDto clientClaim)
        {
            await _clientService.DeleteClientClaimAsync(clientClaim);
            SuccessNotification(_localizer["SuccessDeleteClientClaim"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(ClientClaims), new { Id = clientClaim.ClientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClientPropertyDelete(ClientPropertiesDto clientProperty)
        {
            await _clientService.DeleteClientPropertyAsync(clientProperty);
            SuccessNotification(_localizer["SuccessDeleteClientProperty"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(ClientProperties), new { Id = clientProperty.ClientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApiResourcePropertyDelete(ApiResourcePropertiesDto apiResourceProperty)
        {
            await _apiResourceService.DeleteApiResourcePropertyAsync(apiResourceProperty);
            SuccessNotification(_localizer["SuccessDeleteApiResourceProperty"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(ApiResourceProperties), new { Id = apiResourceProperty.ApiResourceId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IdentityResourcePropertyDelete(IdentityResourcePropertiesDto identityResourceProperty)
        {
            await _identityResourceService.DeleteIdentityResourcePropertyAsync(identityResourceProperty);
            SuccessNotification(_localizer["SuccessDeleteIdentityResourceProperty"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(IdentityResourceProperties), new { Id = identityResourceProperty.IdentityResourceId });
        }

        [HttpGet]
        public async Task<IActionResult> ClientSecrets(int id, int? page)
        {
            if (id == 0) return NotFound();

            var clientSecrets = await _clientService.GetClientSecretsAsync(id, page ?? 1);
            _clientService.BuildClientSecretsViewModel(clientSecrets);

            return View(clientSecrets);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClientSecrets(ClientSecretsDto clientSecret)
        {
            await _clientService.AddClientSecretAsync(clientSecret);
            SuccessNotification(string.Format(_localizer["SuccessAddClientSecret"], clientSecret.ClientName), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(ClientSecrets), new { Id = clientSecret.ClientId });
        }

        [HttpGet]
        public async Task<IActionResult> ClientSecretDelete(int id)
        {
            if (id == 0) return NotFound();

            var clientSecret = await _clientService.GetClientSecretAsync(id);

            return View(nameof(ClientSecretDelete), clientSecret);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClientSecretDelete(ClientSecretsDto clientSecret)
        {
            await _clientService.DeleteClientSecretAsync(clientSecret);
            SuccessNotification(_localizer["SuccessDeleteClientSecret"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(ClientSecrets), new { Id = clientSecret.ClientId });
        }

        [HttpGet]
        public async Task<IActionResult> SearchScopes(string scope, int limit = 0)
        {
            var scopes = await _clientService.GetScopesAsync(scope, limit);

            return Ok(scopes);
        }

        [HttpGet]
        public IActionResult SearchClaims(string claim, int limit = 0)
        {
            var claims = _clientService.GetStandardClaims(claim, limit);

            return Ok(claims);
        }

        [HttpGet]
        public IActionResult SearchGrantTypes(string grant, int limit = 0)
        {
            var grants = _clientService.GetGrantTypes(grant, limit);

            return Ok(grants);
        }

        [HttpGet]
        public async Task<IActionResult> Clients(int? page, string search)
        {
            ViewBag.Search = search;
            return View(await _clientService.GetClientsAsync(search, page ?? 1));
        }

        [HttpGet]
        public async Task<IActionResult> IdentityResourceDelete(int id)
        {
            if (id == 0) return NotFound();

            var identityResource = await _identityResourceService.GetIdentityResourceAsync(id);

            return View(identityResource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IdentityResourceDelete(IdentityResourceDto identityResource)
        {
            await _identityResourceService.DeleteIdentityResourceAsync(identityResource);
            SuccessNotification(_localizer["SuccessDeleteIdentityResource"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(IdentityResources));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IdentityResource(IdentityResourceDto identityResource)
        {
            if (!ModelState.IsValid)
            {
                return View(identityResource);
            }

            identityResource = _identityResourceService.BuildIdentityResourceViewModel(identityResource);

            int identityResourceId;

            if (identityResource.Id == 0)
            {
                identityResourceId = await _identityResourceService.AddIdentityResourceAsync(identityResource);
            }
            else
            {
                identityResourceId = identityResource.Id;
                await _identityResourceService.UpdateIdentityResourceAsync(identityResource);
            }

            SuccessNotification(string.Format(_localizer["SuccessAddIdentityResource"], identityResource.Name), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(IdentityResource), new { Id = identityResourceId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApiResource(ApiResourceDto apiResource)
        {
            if (!ModelState.IsValid)
            {
                return View(apiResource);
            }

            ComboBoxHelpers.PopulateValuesToList(apiResource.UserClaimsItems, apiResource.UserClaims);

            int apiResourceId;

            if (apiResource.Id == 0)
            {
                apiResourceId = await _apiResourceService.AddApiResourceAsync(apiResource);
            }
            else
            {
                apiResourceId = apiResource.Id;
                await _apiResourceService.UpdateApiResourceAsync(apiResource);
            }

            SuccessNotification(string.Format(_localizer["SuccessAddApiResource"], apiResource.Name), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(ApiResource), new { Id = apiResourceId });
        }

        [HttpGet]
        public async Task<IActionResult> ApiResourceDelete(int id)
        {
            if (id == 0) return NotFound();

            var apiResource = await _apiResourceService.GetApiResourceAsync(id);

            return View(apiResource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApiResourceDelete(ApiResourceDto apiResource)
        {
            await _apiResourceService.DeleteApiResourceAsync(apiResource);
            SuccessNotification(_localizer["SuccessDeleteApiResource"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(ApiResources));
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        [Route("[controller]/[action]/{id:int}")]
        public async Task<IActionResult> ApiResource(int id)
        {
            if (id == 0)
            {
                var apiResourceDto = new ApiResourceDto();
                return View(apiResourceDto);
            }

            var apiResource = await _apiResourceService.GetApiResourceAsync(id);

            return View(apiResource);
        }

        [HttpGet]
        public async Task<IActionResult> ApiSecrets(int id, int? page)
        {
            if (id == 0) return NotFound();

            var apiSecrets = await _apiResourceService.GetApiSecretsAsync(id, page ?? 1);
            _apiResourceService.BuildApiSecretsViewModel(apiSecrets);

            return View(apiSecrets);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApiSecrets(ApiSecretsDto apiSecret)
        {
            if (!ModelState.IsValid)
            {
                return View(apiSecret);
            }

            await _apiResourceService.AddApiSecretAsync(apiSecret);
            SuccessNotification(_localizer["SuccessAddApiSecret"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(ApiSecrets), new { Id = apiSecret.ApiResourceId });
        }

        [HttpGet]
        public async Task<IActionResult> ApiScopes(int id, int? page, int? scope)
        {
            if (id == 0 || !ModelState.IsValid) return NotFound();

            if (scope == null)
            {
                var apiScopesDto = await _apiResourceService.GetApiScopesAsync(id, page ?? 1);

                return View(apiScopesDto);
            }
            else
            {
                var apiScopesDto = await _apiResourceService.GetApiScopeAsync(id, scope.Value);
                return View(apiScopesDto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApiScopes(ApiScopesDto apiScope)
        {
            if (!ModelState.IsValid)
            {
                return View(apiScope);
            }

            _apiResourceService.BuildApiScopeViewModel(apiScope);

            int apiScopeId;

            if (apiScope.ApiScopeId == 0)
            {
                apiScopeId = await _apiResourceService.AddApiScopeAsync(apiScope);
            }
            else
            {
                apiScopeId = apiScope.ApiScopeId;
                await _apiResourceService.UpdateApiScopeAsync(apiScope);
            }

            SuccessNotification(string.Format(_localizer["SuccessAddApiScope"], apiScope.Name), _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(ApiScopes), new { Id = apiScope.ApiResourceId, Scope = apiScopeId });
        }

        [HttpGet]
        public async Task<IActionResult> ApiScopeDelete(int id, int scope)
        {
            if (id == 0 || scope == 0) return NotFound();

            var apiScope = await _apiResourceService.GetApiScopeAsync(id, scope);

            return View(nameof(ApiScopeDelete), apiScope);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApiScopeDelete(ApiScopesDto apiScope)
        {
            await _apiResourceService.DeleteApiScopeAsync(apiScope);
            SuccessNotification(_localizer["SuccessDeleteApiScope"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(ApiScopes), new { Id = apiScope.ApiResourceId });
        }

        [HttpGet]
        public async Task<IActionResult> ApiResources(int? page, string search)
        {
            ViewBag.Search = search;
            var apiResources = await _apiResourceService.GetApiResourcesAsync(search, page ?? 1);

            return View(apiResources);
        }

        [HttpGet]
        public async Task<IActionResult> IdentityResources(int? page, string search)
        {
            ViewBag.Search = search;
            var identityResourcesDto = await _identityResourceService.GetIdentityResourcesAsync(search, page ?? 1);

            return View(identityResourcesDto);
        }

        [HttpGet]
        public async Task<IActionResult> ApiSecretDelete(int id)
        {
            if (id == 0) return NotFound();

            var clientSecret = await _apiResourceService.GetApiSecretAsync(id);

            return View(nameof(ApiSecretDelete), clientSecret);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApiSecretDelete(ApiSecretsDto apiSecret)
        {
            await _apiResourceService.DeleteApiSecretAsync(apiSecret);
            SuccessNotification(_localizer["SuccessDeleteApiSecret"], _localizer["SuccessTitle"]);

            return RedirectToAction(nameof(ApiSecrets), new { Id = apiSecret.ApiResourceId });
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        [Route("[controller]/[action]/{id:int}")]
        public async Task<IActionResult> IdentityResource(int id)
        {
            if (id == 0)
            {
                var identityResourceDto = new IdentityResourceDto();
                return View(identityResourceDto);
            }

            var identityResource = await _identityResourceService.GetIdentityResourceAsync(id);

            return View(identityResource);
        }
    }
}