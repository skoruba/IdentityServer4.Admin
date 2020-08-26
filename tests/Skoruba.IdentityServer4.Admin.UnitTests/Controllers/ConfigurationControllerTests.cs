using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.AuditLogging.EntityFramework.Extensions;
using Skoruba.AuditLogging.EntityFramework.Repositories;
using Skoruba.AuditLogging.EntityFramework.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.Controllers;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Skoruba.IdentityServer4.Admin.Helpers;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Controllers
{
    public class ConfigurationControllerTests
    {
        [Fact]
        public async Task GetClients()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var newClient = await GenerateClient(dbContext);

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);

            // Act
            var result = await controller.Clients(page: 1, search: string.Empty);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewName.Should().BeNullOrEmpty();
            viewResult.ViewData.Should().NotBeNull();

            var viewModel = Assert.IsType<ClientsDto>(viewResult.ViewData.Model);
            viewModel.Clients.Should().NotBeNull();
            viewModel.Clients.Should().HaveCount(1);

            viewModel.Clients[0].ClientId.Should().Be(newClient.ClientId);
            viewModel.Clients[0].ClientName.Should().Be(newClient.ClientName);
        }

        [Fact]
        public async Task AddClient()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var clientService = serviceProvider.GetRequiredService<IClientService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var clientDto = ClientDtoMock.GenerateRandomClient(0);
            var result = await controller.Client(clientDto);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be(nameof(Client));

            var client = await dbContext.Clients.Where(x => x.ClientId == clientDto.ClientId).SingleOrDefaultAsync();
            var adddedClient = await clientService.GetClientAsync(client.Id);

            clientDto.ShouldBeEquivalentTo(adddedClient, opts => opts.Excluding(x => x.Id)
                .Excluding(x => x.AccessTokenTypes)
                .Excluding(x => x.ProtocolTypes)
                .Excluding(x => x.RefreshTokenExpirations)
                .Excluding(x => x.RefreshTokenUsages));
        }

        [Fact]
        public async Task UpdateClient()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var clientService = serviceProvider.GetRequiredService<IClientService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);

            var clientToAdd = ClientMock.GenerateRandomClient(0);
            await dbContext.Clients.AddAsync(clientToAdd);
            await dbContext.SaveChangesAsync();
            dbContext.Entry(clientToAdd).State = EntityState.Detached;

            var clientDto = ClientDtoMock.GenerateRandomClient(clientToAdd.Id);
            var result = await controller.Client(clientDto);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("Client");

            var client = await dbContext.Clients.Where(x => x.ClientId == clientDto.ClientId).SingleOrDefaultAsync();
            var adddedClient = await clientService.GetClientAsync(client.Id);

            clientDto.ShouldBeEquivalentTo(adddedClient, opts => opts.Excluding(x => x.Id)
                .Excluding(x => x.AccessTokenTypes)
                .Excluding(x => x.ProtocolTypes)
                .Excluding(x => x.RefreshTokenExpirations)
                .Excluding(x => x.RefreshTokenUsages));
        }

        [Fact]
        public async Task DeleteClient()
        {
            //Get Services
            var serviceProvider = GetServices();

            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var client = ClientMock.GenerateRandomClient(0);
            await dbContext.Clients.AddAsync(client);
            await dbContext.SaveChangesAsync();
            dbContext.Entry(client).State = EntityState.Detached;

            var clientDto = ClientDtoMock.GenerateRandomClient(client.Id);
            var result = await controller.ClientDelete(clientDto);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("Clients");

            var deletedClient = await dbContext.Clients.Where(x => x.Id == clientDto.Id).SingleOrDefaultAsync();
            deletedClient.Should().BeNull();
        }

        [Fact]
        public async Task AddClientClaim()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var clientService = serviceProvider.GetRequiredService<IClientService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var clientDto = ClientDtoMock.GenerateRandomClient(0);
            var clientId = await clientService.AddClientAsync(clientDto);
            var clientClaim = ClientDtoMock.GenerateRandomClientClaim(0, clientId);
            var result = await controller.ClientClaims(clientClaim);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("ClientClaims");

            var clientClaimAdded = await dbContext.ClientClaims.Where(x => x.Client.Id == clientId).SingleOrDefaultAsync();
            var adddedClientClaim = await clientService.GetClientClaimAsync(clientClaimAdded.Id);

            clientClaim.ShouldBeEquivalentTo(adddedClientClaim, opts => opts.Excluding(x => x.ClientClaimId)
                        .Excluding(x => x.ClientClaims)
                        .Excluding(x => x.ClientName));
        }

        [Fact]
        public async Task GetClientClaim()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var clientService = serviceProvider.GetRequiredService<IClientService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var clientDto = ClientDtoMock.GenerateRandomClient(0);
            var clientId = await clientService.AddClientAsync(clientDto);
            var clientClaim = ClientDtoMock.GenerateRandomClientClaim(0, clientId);
            await clientService.AddClientClaimAsync(clientClaim);
            var clientClaimAdded = await dbContext.ClientClaims.Where(x => x.Client.Id == clientId).SingleOrDefaultAsync();
            clientClaim.ClientClaimId = clientClaimAdded.Id;

            var result = await controller.ClientClaims(clientId, page: 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewName.Should().BeNullOrEmpty();
            viewResult.ViewData.Should().NotBeNull();

            var viewModel = Assert.IsType<ClientClaimsDto>(viewResult.ViewData.Model);
            viewModel.ClientClaims.Count.Should().Be(1);
            viewModel.ClientClaims[0].ShouldBeEquivalentTo(clientClaimAdded);
        }

        [Fact]
        public async Task DeleteClientClaim()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var clientService = serviceProvider.GetRequiredService<IClientService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var clientDto = ClientDtoMock.GenerateRandomClient(0);
            var clientId = await clientService.AddClientAsync(clientDto);
            var clientClaim = ClientDtoMock.GenerateRandomClientClaim(0, clientId);
            await clientService.AddClientClaimAsync(clientClaim);

            var clientClaimAdded = await dbContext.ClientClaims.Where(x => x.Client.Id == clientId).SingleOrDefaultAsync();
            clientClaim.ClientClaimId = clientClaimAdded.Id;
            dbContext.Entry(clientClaimAdded).State = EntityState.Detached;

            var result = await controller.ClientClaimDelete(clientClaim);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("ClientClaims");

            var clientClaimDelete = await dbContext.ClientClaims.Where(x => x.Id == clientClaimAdded.Id).SingleOrDefaultAsync();
            clientClaimDelete.Should().BeNull();
        }

        [Fact]
        public async Task AddClientSecret()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var clientService = serviceProvider.GetRequiredService<IClientService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var clientDto = ClientDtoMock.GenerateRandomClient(0);
            var clientId = await clientService.AddClientAsync(clientDto);
            var clientSecret = ClientDtoMock.GenerateRandomClientSecret(0, clientId);
            var result = await controller.ClientSecrets(clientSecret);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("ClientSecrets");

            var clientSecretAdded = await dbContext.ClientSecrets.Where(x => x.Client.Id == clientId).SingleOrDefaultAsync();
            var newClientSecret = await clientService.GetClientSecretAsync(clientSecretAdded.Id);

            clientSecret.ShouldBeEquivalentTo(newClientSecret, opts => opts.Excluding(x => x.ClientSecretId)
                        .Excluding(x => x.ClientSecrets)
                        .Excluding(x => x.ClientName));
        }

        [Fact]
        public async Task GetClientSecret()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var clientService = serviceProvider.GetRequiredService<IClientService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var clientDto = ClientDtoMock.GenerateRandomClient(0);
            var clientId = await clientService.AddClientAsync(clientDto);
            var clientSecret = ClientDtoMock.GenerateRandomClientSecret(0, clientId);
            await clientService.AddClientSecretAsync(clientSecret);
            var clientSecretAdded = await dbContext.ClientSecrets.Where(x => x.Client.Id == clientId).SingleOrDefaultAsync();
            clientSecret.ClientSecretId = clientSecretAdded.Id;

            var result = await controller.ClientSecrets(clientId, page: 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewName.Should().BeNullOrEmpty();
            viewResult.ViewData.Should().NotBeNull();

            var viewModel = Assert.IsType<ClientSecretsDto>(viewResult.ViewData.Model);
            viewModel.ClientSecrets.Count.Should().Be(1);
            viewModel.ClientSecrets[0].ShouldBeEquivalentTo(clientSecretAdded);
        }

        [Fact]
        public async Task DeleteClientSecret()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var clientService = serviceProvider.GetRequiredService<IClientService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var clientDto = ClientDtoMock.GenerateRandomClient(0);
            var clientId = await clientService.AddClientAsync(clientDto);
            var clientSecret = ClientDtoMock.GenerateRandomClientSecret(0, clientId);
            await clientService.AddClientSecretAsync(clientSecret);

            var clientSecretAdded = await dbContext.ClientSecrets.Where(x => x.Client.Id == clientId).SingleOrDefaultAsync();
            clientSecret.ClientSecretId = clientSecretAdded.Id;
            dbContext.Entry(clientSecretAdded).State = EntityState.Detached;

            var result = await controller.ClientSecretDelete(clientSecret);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("ClientSecrets");

            var clientSecretDelete = await dbContext.ClientSecrets.Where(x => x.Id == clientSecretAdded.Id).SingleOrDefaultAsync();
            clientSecretDelete.Should().BeNull();
        }

        [Fact]
        public async Task AddClientProperty()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var clientService = serviceProvider.GetRequiredService<IClientService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var clientDto = ClientDtoMock.GenerateRandomClient(0);
            var clientId = await clientService.AddClientAsync(clientDto);
            var clientProperty = ClientDtoMock.GenerateRandomClientProperty(0, clientId);
            var result = await controller.ClientProperties(clientProperty);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("ClientProperties");

            var clientPropertyAdded = await dbContext.ClientProperties.Where(x => x.Client.Id == clientId).SingleOrDefaultAsync();
            var newClientProperty = await clientService.GetClientPropertyAsync(clientPropertyAdded.Id);

            clientProperty.ShouldBeEquivalentTo(newClientProperty, opts => opts.Excluding(x => x.ClientPropertyId)
                        .Excluding(x => x.ClientProperties)
                        .Excluding(x => x.ClientName));
        }

        [Fact]
        public async Task GetClientProperty()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var clientService = serviceProvider.GetRequiredService<IClientService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var clientDto = ClientDtoMock.GenerateRandomClient(0);
            var clientId = await clientService.AddClientAsync(clientDto);
            var clientProperty = ClientDtoMock.GenerateRandomClientProperty(0, clientId);
            await clientService.AddClientPropertyAsync(clientProperty);
            var clientPropertyAdded = await dbContext.ClientProperties.Where(x => x.Client.Id == clientId).SingleOrDefaultAsync();
            clientProperty.ClientPropertyId = clientPropertyAdded.Id;

            var result = await controller.ClientProperties(clientId, page: 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewName.Should().BeNullOrEmpty();
            viewResult.ViewData.Should().NotBeNull();

            var viewModel = Assert.IsType<ClientPropertiesDto>(viewResult.ViewData.Model);
            viewModel.ClientProperties.Count.Should().Be(1);
            viewModel.ClientProperties[0].ShouldBeEquivalentTo(clientPropertyAdded);
        }

        [Fact]
        public async Task DeleteClientProperty()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var clientService = serviceProvider.GetRequiredService<IClientService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var clientDto = ClientDtoMock.GenerateRandomClient(0);
            var clientId = await clientService.AddClientAsync(clientDto);
            var clientProperty = ClientDtoMock.GenerateRandomClientProperty(0, clientId);
            await clientService.AddClientPropertyAsync(clientProperty);

            var clientPropertyAdded = await dbContext.ClientProperties.Where(x => x.Client.Id == clientId).SingleOrDefaultAsync();
            clientProperty.ClientPropertyId = clientPropertyAdded.Id;
            dbContext.Entry(clientPropertyAdded).State = EntityState.Detached;
            var result = await controller.ClientPropertyDelete(clientProperty);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("ClientProperties");

            var newClientProperty = await dbContext.ClientProperties.Where(x => x.Id == clientPropertyAdded.Id).SingleOrDefaultAsync();
            newClientProperty.Should().BeNull();
        }

        [Fact]
        public async Task AddClientModelIsNotValid()
        {
            //Get Services
            var serviceProvider = GetServices();
            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);

            //Create empty dto object
            var clientDto = new ClientDto();

            //Setup requirements for model validation
            controller.ModelState.AddModelError("ClientId", "Required");
            controller.ModelState.AddModelError("ClientName", "Required");

            //Action
            var result = await controller.Client(clientDto);

            // Assert            
            var viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task AddIdentityResource()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var identityResourceService = serviceProvider.GetRequiredService<IIdentityResourceService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var identityResourceDto = IdentityResourceDtoMock.GenerateRandomIdentityResource(0);
            var result = await controller.IdentityResource(identityResourceDto);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("IdentityResource");

            var identityResource = await dbContext.IdentityResources.Where(x => x.Name == identityResourceDto.Name).SingleOrDefaultAsync();
            var addedIdentityResource = await identityResourceService.GetIdentityResourceAsync(identityResource.Id);

            identityResourceDto.ShouldBeEquivalentTo(addedIdentityResource, opts => opts.Excluding(x => x.Id));
        }

        [Fact]
        public async Task DeleteIdentityResource()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var identityResourceService = serviceProvider.GetRequiredService<IIdentityResourceService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var identityResourceDto = IdentityResourceDtoMock.GenerateRandomIdentityResource(0);
            await identityResourceService.AddIdentityResourceAsync(identityResourceDto);

            var identityResourceId = await dbContext.IdentityResources.Where(x => x.Name == identityResourceDto.Name).Select(x => x.Id).SingleOrDefaultAsync();

            identityResourceId.Should().NotBe(0);

            identityResourceDto.Id = identityResourceId;

            var result = await controller.IdentityResourceDelete(identityResourceDto);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("IdentityResources");

            var identityResource = await dbContext.IdentityResources.Where(x => x.Id == identityResourceDto.Id).SingleOrDefaultAsync();
            identityResource.Should().BeNull();
        }

        [Fact]
        public async Task AddApiResource()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var apiResourceService = serviceProvider.GetRequiredService<IApiResourceService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);
            var result = await controller.ApiResource(apiResourceDto);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("ApiResource");

            var apiResource = await dbContext.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();
            var addedApiResource = await apiResourceService.GetApiResourceAsync(apiResource.Id);

            apiResourceDto.ShouldBeEquivalentTo(addedApiResource, opts => opts.Excluding(x => x.Id));
        }

        [Fact]
        public async Task DeleteApiResource()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var apiResourceService = serviceProvider.GetRequiredService<IApiResourceService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);
            await apiResourceService.AddApiResourceAsync(apiResourceDto);
            var apiResourceId = await dbContext.ApiResources.Where(x => x.Name == apiResourceDto.Name).Select(x => x.Id).SingleOrDefaultAsync();

            apiResourceId.Should().NotBe(0);

            apiResourceDto.Id = apiResourceId;
            var result = await controller.ApiResourceDelete(apiResourceDto);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("ApiResources");

            var apiResource = await dbContext.ApiResources.Where(x => x.Id == apiResourceDto.Id).SingleOrDefaultAsync();
            apiResource.Should().BeNull();
        }

        [Fact]
        public async Task AddApiScope()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var apiResourceService = serviceProvider.GetRequiredService<IApiResourceService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);
            await apiResourceService.AddApiResourceAsync(apiResourceDto);
            var resource = await dbContext.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();
            var apiScopeDto = ApiResourceDtoMock.GenerateRandomApiScope(0, resource.Id);

            var result = await controller.ApiScopes(apiScopeDto);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("ApiScopes");

            var apiScope = await dbContext.ApiScopes.Where(x => x.Name == apiScopeDto.Name).SingleOrDefaultAsync();
            var addedApiScope = await apiResourceService.GetApiScopeAsync(resource.Id, apiScope.Id);

            apiScopeDto.ShouldBeEquivalentTo(addedApiScope, opts => opts.Excluding(x => x.ApiResourceId).Excluding(x => x.ResourceName).Excluding(x => x.ApiScopeId));
        }

        [Fact]
        public async Task GetApiScopes()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var apiResourceService = serviceProvider.GetRequiredService<IApiResourceService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);

            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);
            await apiResourceService.AddApiResourceAsync(apiResourceDto);

            var resource = await dbContext.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

            const int generateScopes = 5;

            // Add Api Scopes
            for (var i = 0; i < generateScopes; i++)
            {
                var apiScopeDto = ApiResourceDtoMock.GenerateRandomApiScope(0, resource.Id);
                await apiResourceService.AddApiScopeAsync(apiScopeDto);
            }

            var result = await controller.ApiScopes(resource.Id, 1, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewName.Should().BeNullOrEmpty();
            viewResult.ViewData.Should().NotBeNull();

            var viewModel = Assert.IsType<ApiScopesDto>(viewResult.ViewData.Model);
            viewModel.Scopes.Count.Should().Be(generateScopes);
        }

        [Fact]
        public async Task UpdateApiScope()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var apiResourceService = serviceProvider.GetRequiredService<IApiResourceService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);
            await apiResourceService.AddApiResourceAsync(apiResourceDto);
            var resource = await dbContext.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();
            var apiScopeDto = ApiResourceDtoMock.GenerateRandomApiScope(0, resource.Id);

            await apiResourceService.AddApiScopeAsync(apiScopeDto);
            var apiScopeAdded = await dbContext.ApiScopes.Where(x => x.Name == apiScopeDto.Name).SingleOrDefaultAsync();

            dbContext.Entry(apiScopeAdded).State = EntityState.Detached;

            apiScopeAdded.Should().NotBeNull();

            var updatedApiScopeDto = ApiResourceDtoMock.GenerateRandomApiScope(apiScopeAdded.Id, resource.Id);
            var result = await controller.ApiScopes(updatedApiScopeDto);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("ApiScopes");

            var apiScope = await dbContext.ApiScopes.Where(x => x.Id == apiScopeAdded.Id).SingleOrDefaultAsync();
            var addedApiScope = await apiResourceService.GetApiScopeAsync(resource.Id, apiScope.Id);

            updatedApiScopeDto.ShouldBeEquivalentTo(addedApiScope, opts => opts.Excluding(x => x.ApiResourceId)
                .Excluding(x => x.ResourceName)
                .Excluding(x => x.ApiScopeId));
        }

        [Fact]
        public async Task DeleteApiScope()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var apiResourceService = serviceProvider.GetRequiredService<IApiResourceService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);
            await apiResourceService.AddApiResourceAsync(apiResourceDto);
            var resource = await dbContext.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();
            var apiScopeDto = ApiResourceDtoMock.GenerateRandomApiScope(0, resource.Id);
            await apiResourceService.AddApiScopeAsync(apiScopeDto);

            var apiScopeId = await dbContext.ApiScopes.Where(x => x.Name == apiScopeDto.Name).Select(x => x.Id).SingleOrDefaultAsync();

            apiScopeId.Should().NotBe(0);

            apiScopeDto.ApiScopeId = apiScopeId;

            var result = await controller.ApiScopeDelete(apiScopeDto);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("ApiScopes");

            var apiScope = await dbContext.ApiScopes.Where(x => x.Id == apiScopeDto.ApiScopeId).SingleOrDefaultAsync();
            apiScope.Should().BeNull();
        }

        [Fact]
        public async Task GetApiSecrets()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var apiResourceService = serviceProvider.GetRequiredService<IApiResourceService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);
            await apiResourceService.AddApiResourceAsync(apiResourceDto);
            var resource = await dbContext.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

            const int generateApiSecrets = 5;

            for (var i = 0; i < generateApiSecrets; i++)
            {
                var apiSecretsDto = ApiResourceDtoMock.GenerateRandomApiSecret(0, resource.Id);
                await apiResourceService.AddApiSecretAsync(apiSecretsDto);
            }

            var result = await controller.ApiSecrets(resource.Id, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewName.Should().BeNullOrEmpty();
            viewResult.ViewData.Should().NotBeNull();

            var viewModel = Assert.IsType<ApiSecretsDto>(viewResult.ViewData.Model);
            viewModel.ApiSecrets.Count.Should().Be(generateApiSecrets);
        }

        [Fact]
        public async Task AddApiSecret()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var apiResourceService = serviceProvider.GetRequiredService<IApiResourceService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);
            await apiResourceService.AddApiResourceAsync(apiResourceDto);
            var resource = await dbContext.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();
            var apiSecretsDto = ApiResourceDtoMock.GenerateRandomApiSecret(0, resource.Id);

            var result = await controller.ApiSecrets(apiSecretsDto);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("ApiSecrets");

            var apiSecret = await dbContext.ApiSecrets.Where(x => x.Value == apiSecretsDto.Value).SingleOrDefaultAsync();
            var addedApiScope = await apiResourceService.GetApiSecretAsync(apiSecret.Id);

            apiSecretsDto.ShouldBeEquivalentTo(addedApiScope, opts => opts.Excluding(x => x.ApiResourceId).Excluding(x => x.ApiResourceName).Excluding(x => x.ApiSecretId));
        }

        [Fact]
        public async Task DeleteApiSecret()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<IdentityServerConfigurationDbContext>();
            var apiResourceService = serviceProvider.GetRequiredService<IApiResourceService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);
            await apiResourceService.AddApiResourceAsync(apiResourceDto);
            var resource = await dbContext.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();
            var apiSecretsDto = ApiResourceDtoMock.GenerateRandomApiSecret(0, resource.Id);
            await apiResourceService.AddApiSecretAsync(apiSecretsDto);

            var apiSecretId = await dbContext.ApiSecrets.Where(x => x.Value == apiSecretsDto.Value).Select(x => x.Id)
                .SingleOrDefaultAsync();

            apiSecretId.Should().NotBe(0);

            apiSecretsDto.ApiSecretId = apiSecretId;
            var result = await controller.ApiSecretDelete(apiSecretsDto);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("ApiSecrets");

            var apiSecret = await dbContext.ApiSecrets.Where(x => x.Id == apiSecretsDto.ApiSecretId).SingleOrDefaultAsync();
            apiSecret.Should().BeNull();
        }

        private ConfigurationController PrepareConfigurationController(IServiceProvider serviceProvider)
        {
            // Arrange
            var identityResourceService = serviceProvider.GetRequiredService<IIdentityResourceService>();
            var apiResourceService = serviceProvider.GetRequiredService<IApiResourceService>();
            var clientService = serviceProvider.GetRequiredService<IClientService>();
            var localizer = serviceProvider.GetRequiredService<IStringLocalizer<ConfigurationController>>();
            var logger = serviceProvider.GetRequiredService<ILogger<ConfigurationController>>();
            var tempDataDictionaryFactory = serviceProvider.GetRequiredService<ITempDataDictionaryFactory>();

            //Get Controller
            var controller = new ConfigurationController(identityResourceService, apiResourceService, clientService, localizer, logger);

            //Setup TempData for notofication in basecontroller
            var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            var tempData = tempDataDictionaryFactory.GetTempData(httpContext);
            controller.TempData = tempData;

            return controller;
        }

        private async Task<Client> GenerateClient(IdentityServerConfigurationDbContext dbContext)
        {
            var client = ClientMock.GenerateRandomClient(id: 0);

            await dbContext.Clients.AddAsync(client);
            await dbContext.SaveChangesAsync();

            return client;
        }

        private IServiceProvider GetServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

            //Entity framework
            var efServiceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
            services.AddOptions();
            services.AddDbContext<IdentityServerConfigurationDbContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(efServiceProvider));
            services.AddDbContext<AdminAuditLogDbContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(efServiceProvider));

            //Http Context
            var context = new DefaultHttpContext();
            services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = context });

            //IdentityServer4 EntityFramework configuration
            services.AddSingleton<ConfigurationStoreOptions>();
            services.AddSingleton<OperationalStoreOptions>();

            //Audit logging
            services.AddAuditLogging()
                .AddDefaultEventData()
                .AddAuditSinks<DatabaseAuditEventLoggerSink<AuditLog>>();
            services.AddTransient<IAuditLoggingRepository<AuditLog>, AuditLoggingRepository<AdminAuditLogDbContext, AuditLog>>();
            
            //Add Admin services
            services.AddMvcExceptionFilters();

            services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();

            services.AddAdminAspNetIdentityServices<AdminIdentityDbContext, IdentityServerPersistedGrantDbContext, UserDto<string>, RoleDto<string>,
                UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
                UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
                UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
                UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>,
                RoleClaimsDto<string>, UserClaimDto<string>, RoleClaimDto<string>>();

            services.AddSession();
            services.AddMvc()
            .AddViewLocalization(
                LanguageViewLocationExpanderFormat.Suffix,
                opts => { opts.ResourcesPath = "Resources"; })
            .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supportedCultures = new[]
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("en")
                    };

                    opts.DefaultRequestCulture = new RequestCulture("en");
                    opts.SupportedCultures = supportedCultures;
                    opts.SupportedUICultures = supportedCultures;
                });

            services.AddLogging();

            return services.BuildServiceProvider();
        }
    }
}