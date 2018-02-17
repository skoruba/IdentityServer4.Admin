using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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
using Skoruba.IdentityServer4.Admin.Controllers;
using Skoruba.IdentityServer4.Admin.Data.DbContexts;
using Skoruba.IdentityServer4.Admin.Data.Repositories;
using Skoruba.IdentityServer4.Admin.Services;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Skoruba.IdentityServer4.Admin.ViewModels.Configuration;
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
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
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
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
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
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
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
            viewResult.ActionName.Should().Be("Clients");

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

            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();

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
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
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
                        .Excluding(x => x.ClientClaims));
        }

        [Fact]
        public async Task GetClientClaim()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
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
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
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
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
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
                        .Excluding(x => x.ClientSecrets));
        }

        [Fact]
        public async Task GetClientSecret()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
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
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
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
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
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
                        .Excluding(x => x.ClientProperties));
        }

        [Fact]
        public async Task GetClientProperty()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
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
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
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
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityResourceService = serviceProvider.GetRequiredService<IIdentityResourceService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var identityResourceDto = IdentityResourceDtoMock.GenerateRandomIdentityResource(0);
            var result = await controller.IdentityResource(identityResourceDto);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("IdentityResources");

            var identityResource = await dbContext.IdentityResources.Where(x => x.Name == identityResourceDto.Name).SingleOrDefaultAsync();
            var addedIdentityResource = await identityResourceService.GetIdentityResourceAsync(identityResource.Id);

            identityResourceDto.ShouldBeEquivalentTo(addedIdentityResource, opts => opts.Excluding(x => x.Id));
        }

        [Fact]
        public async Task AddApiResource()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var apiResourceService = serviceProvider.GetRequiredService<IApiResourceService>();

            // Get controller
            var controller = PrepareConfigurationController(serviceProvider);
            var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);
            var result = await controller.ApiResource(apiResourceDto);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("ApiResources");

            var apiResource = await dbContext.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();
            var addedApiResource = await apiResourceService.GetApiResourceAsync(apiResource.Id);

            apiResourceDto.ShouldBeEquivalentTo(addedApiResource, opts => opts.Excluding(x => x.Id));
        }

        [Fact]
        public async Task AddApiScope()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
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
        public async Task AddApiSecret()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
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

            apiSecretsDto.ShouldBeEquivalentTo(addedApiScope, opts => opts.Excluding(x => x.ApiResourceId).Excluding(x=> x.ApiResourceName).Excluding(x => x.ApiSecretId));
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

        private async Task<Client> GenerateClient(AdminDbContext dbContext)
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
            services.AddDbContext<AdminDbContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(efServiceProvider));

            //Http Context
            var context = new DefaultHttpContext();
            services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = context });

            //IdentityServer4 EntityFramework configuration
            services.AddSingleton<ConfigurationStoreOptions>();
            services.AddSingleton<OperationalStoreOptions>();

            //Repositories
            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IIdentityResourceRepository, IdentityResourceRepository>();
            services.AddTransient<IIdentityRepository, IdentityRepository>();
            services.AddTransient<IApiResourceRepository, ApiResourceRepository>();
            services.AddTransient<IPersistedGrantRepository, PersistedGrantRepository>();
            services.AddTransient<ILogRepository, LogRepository>();

            //Services
            services.AddTransient<ILogService, LogService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IApiResourceService, ApiResourceService>();
            services.AddTransient<IIdentityResourceService, IdentityResourceService>();
            services.AddTransient<IPersistedGrantService, PersistedGrantService>();
            services.AddTransient<IIdentityService, IdentityService>();

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