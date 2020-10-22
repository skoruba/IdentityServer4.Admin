using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Moq;
using Skoruba.AuditLogging.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Services
{
	public class ApiResourceServiceTests
	{
		public ApiResourceServiceTests()
		{
			var databaseName = Guid.NewGuid().ToString();

			_dbContextOptions = new DbContextOptionsBuilder<IdentityServerConfigurationDbContext>()
				.UseInMemoryDatabase(databaseName)
				.Options;

			_storeOptions = new ConfigurationStoreOptions();
			_operationalStore = new OperationalStoreOptions();
		}

		private readonly DbContextOptions<IdentityServerConfigurationDbContext> _dbContextOptions;
		private readonly ConfigurationStoreOptions _storeOptions;
		private readonly OperationalStoreOptions _operationalStore;

		private IClientRepository GetClientRepository(IdentityServerConfigurationDbContext context)
		{
			IClientRepository clientRepository = new ClientRepository<IdentityServerConfigurationDbContext>(context);

			return clientRepository;
		}

		private IApiResourceRepository GetApiResourceRepository(IdentityServerConfigurationDbContext context)
		{
			IApiResourceRepository apiResourceRepository = new ApiResourceRepository<IdentityServerConfigurationDbContext>(context);

			return apiResourceRepository;
		}

		private IClientService GetClientService(IClientRepository repository, IClientServiceResources resources, IAuditEventLogger auditEventLogger)
		{
			IClientService clientService = new ClientService(repository, resources, auditEventLogger);

			return clientService;
		}

		private IApiResourceService GetApiResourceService(IApiResourceRepository repository, IApiResourceServiceResources resources, IClientService clientService, IAuditEventLogger auditEventLogger)
		{
			IApiResourceService apiResourceService = new ApiResourceService(repository, resources, clientService, auditEventLogger);

			return apiResourceService;
		}

        private IApiResourceService GetApiResourceService(IdentityServerConfigurationDbContext context)
        {
            var apiResourceRepository = GetApiResourceRepository(context);
            var clientRepository = GetClientRepository(context);

            var localizerApiResourceMock = new Mock<IApiResourceServiceResources>();
            var localizerApiResource = localizerApiResourceMock.Object;

            var localizerClientResourceMock = new Mock<IClientServiceResources>();
            var localizerClientResource = localizerClientResourceMock.Object;

            var auditLoggerMock = new Mock<IAuditEventLogger>();
            var auditLogger = auditLoggerMock.Object;

            var clientService = GetClientService(clientRepository, localizerClientResource, auditLogger);
            var apiResourceService = GetApiResourceService(apiResourceRepository, localizerApiResource, clientService, auditLogger);

            return apiResourceService;
        }

        [Fact]
		public async Task AddApiResourceAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceService = GetApiResourceService(context);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

				await apiResourceService.AddApiResourceAsync(apiResourceDto);

				//Get new api resource
				var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

				var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

				//Assert new api resource
				apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));
			}
		}

		[Fact]
		public async Task GetApiResourceAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
			{
                var apiResourceService = GetApiResourceService(context);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

				await apiResourceService.AddApiResourceAsync(apiResourceDto);

				//Get new api resource
				var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

				var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

				//Assert new api resource
				apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));
			}
		}

		[Fact]
		public async Task RemoveApiResourceAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
			{
                var apiResourceService = GetApiResourceService(context);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

				await apiResourceService.AddApiResourceAsync(apiResourceDto);

				//Get new api resource
				var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

				var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

				//Assert new api resource
				apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

				//Remove api resource
				await apiResourceService.DeleteApiResourceAsync(newApiResourceDto);

				//Try get removed api resource
				var removeApiResource = await context.ApiResources.Where(x => x.Id == apiResource.Id)
					.SingleOrDefaultAsync();

				//Assert removed api resource
				removeApiResource.Should().BeNull();
			}
		}

		[Fact]
		public async Task UpdateApiResourceAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
			{
                var apiResourceService = GetApiResourceService(context);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

				await apiResourceService.AddApiResourceAsync(apiResourceDto);

				//Get new api resource
				var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

				var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

				//Assert new api resource
				apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

				//Detached the added item
				context.Entry(apiResource).State = EntityState.Detached;

				//Generete new api resuorce with added item id
				var updatedApiResource = ApiResourceDtoMock.GenerateRandomApiResource(apiResource.Id);

				//Update api resource
				await apiResourceService.UpdateApiResourceAsync(updatedApiResource);

				var updatedApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

				//Assert updated api resuorce
				updatedApiResource.Should().BeEquivalentTo(updatedApiResourceDto, options => options.Excluding(o => o.Id));
			}
		}

		[Fact]
		public async Task AddApiScopeAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
			{
                var apiResourceService = GetApiResourceService(context);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

				await apiResourceService.AddApiResourceAsync(apiResourceDto);

				//Get new api resource
				var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

				var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

				//Assert new api resource
				apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

				//Generate random new api scope
				var apiScopeDtoMock = ApiResourceDtoMock.GenerateRandomApiScope(0, newApiResourceDto.Id);

				//Add new api scope
				await apiResourceService.AddApiScopeAsync(newApiResourceDto.Id, apiScopeDtoMock);

				//Get inserted api scope
				var apiScope = await context.ApiScopes.SingleOrDefaultAsync(x => x.Name == apiScopeDtoMock.Name);

				//Map entity to model
				var apiScopesDto = apiScope.ToModel();

				//Get new api scope
				var newApiScope = await apiResourceService.GetApiScopeAsync(newApiResourceDto.Id, apiScopesDto.ApiScopeId);

				//Assert
				newApiScope.Should().BeEquivalentTo(apiScopesDto, o =>
					o.Excluding(x => x.ShowInDiscoveryDocument)
					 .Excluding(x => x.ApiResourceId)
					 .Excluding(x => x.ResourceName)
					 .Excluding(x => x.Description)
					 .Excluding(x => x.DisplayName)
					 .Excluding(x => x.PageSize)
					 .Excluding(x => x.TotalCount)
					 .Excluding(x => x.ApiScopeId)
					 .Excluding(x => x.Required)
					 .Excluding(x => x.Emphasize)
					 .Excluding(x => x.Scopes)
					 .Excluding(x => x.Name)
					 .Excluding(x => x.UserClaims));
			}
		}

		[Fact]
		public async Task GetApiScopeAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
			{
                var apiResourceService = GetApiResourceService(context);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

				await apiResourceService.AddApiResourceAsync(apiResourceDto);

				//Get new api resource
				var apiResource = await context.ApiResources.SingleOrDefaultAsync(x => x.Name == apiResourceDto.Name);

				var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

				//Assert new api resource
				apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

				//Generate random new api scope
				var apiScopeDtoMock = ApiResourceDtoMock.GenerateRandomApiScope(0, newApiResourceDto.Id);

				//Add new api scope
				await apiResourceService.AddApiScopeAsync(newApiResourceDto.Id, apiScopeDtoMock);

				//Get inserted api scope
				var apiScope = await context.ApiScopes.FirstOrDefaultAsync(x => x.Name == apiScopeDtoMock.Name && x.Id == newApiResourceDto.Id);

				//Map entity to model
				var apiScopesDto = apiScope.ToModel();

				//Get new api scope
				var newApiScope = await apiResourceService.GetApiScopeAsync(newApiResourceDto.Id, apiScopesDto.ApiScopeId);

				//Assert
				newApiScope.Should().BeEquivalentTo(apiScopesDto, o =>
					o.Excluding(x => x.ShowInDiscoveryDocument)
					 .Excluding(x => x.ApiResourceId)
					 .Excluding(x => x.ResourceName)
					 .Excluding(x => x.Description)
					 .Excluding(x => x.DisplayName)
					 .Excluding(x => x.UserClaims)
					 .Excluding(x => x.PageSize)
					 .Excluding(x => x.TotalCount)
					 .Excluding(x => x.ApiScopeId)
					 .Excluding(x => x.Required)
					 .Excluding(x => x.Emphasize)
					 .Excluding(x => x.Scopes)
					 .Excluding(x => x.Name)
				);
			}
		}

		[Fact]
		public async Task UpdateApiScopeAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
			{
                var apiResourceService = GetApiResourceService(context);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

				await apiResourceService.AddApiResourceAsync(apiResourceDto);

				//Get new api resource
				var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

				var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

				//Assert new api resource
				apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

				//Generate random new api scope
				var apiScopeDtoMock = ApiResourceDtoMock.GenerateRandomApiScope(0, newApiResourceDto.Id);

				//Add new api scope
				await apiResourceService.AddApiScopeAsync(newApiResourceDto.Id, apiScopeDtoMock);

				//Get inserted api scope
				var apiScope = await context.ApiScopes
											.FirstOrDefaultAsync(x => x.Name == apiScopeDtoMock.Name && x.Id == newApiResourceDto.Id);

				//Map entity to model
				var apiScopeDto = apiScope.ToModel();

				//Get new api scope
				var newApiScope = await apiResourceService.GetApiScopeAsync(newApiResourceDto.Id, apiScopeDto.ApiScopeId);

				//Assert
				newApiScope.Should().BeEquivalentTo(apiScopeDto, o =>
					o.Excluding(x => x.ShowInDiscoveryDocument)
					 .Excluding(x => x.ApiResourceId)
					 .Excluding(x => x.ResourceName)
					 .Excluding(x => x.Description)
					 .Excluding(x => x.DisplayName)
					 .Excluding(x => x.PageSize)
					 .Excluding(x => x.TotalCount)
					 .Excluding(x => x.ApiScopeId)
					 .Excluding(x => x.Required)
					 .Excluding(x => x.Emphasize)
					 .Excluding(x => x.Scopes)
					 .Excluding(x => x.Name)
					 .Excluding(x => x.UserClaims)
				);

				//Detached the added item
				context.Entry(apiScope).State = EntityState.Detached;

				//Update api scope
				var updatedApiScope = ApiResourceDtoMock.GenerateRandomApiScope(newApiResourceDto.Id, apiScopeDto.ApiScopeId);

				await apiResourceService.UpdateApiScopeAsync(updatedApiScope);

				var updatedApiScopeDto = await apiResourceService.GetApiScopeAsync(newApiResourceDto.Id, apiScopeDto.ApiScopeId);

				//Assert updated api scope
				updatedApiScope.Should().BeEquivalentTo(updatedApiScopeDto, o =>
					o.Excluding(x => x.ShowInDiscoveryDocument)
					 .Excluding(x => x.ApiResourceId)
					 .Excluding(x => x.ResourceName)
					 .Excluding(x => x.Description)
					 .Excluding(x => x.DisplayName)
					 .Excluding(x => x.PageSize)
					 .Excluding(x => x.TotalCount)
					 .Excluding(x => x.ApiScopeId)
					 .Excluding(x => x.Required)
					 .Excluding(x => x.Emphasize)
					 .Excluding(x => x.Scopes)
					 .Excluding(x => x.Name)
					 .Excluding(x => x.UserClaims)
				);
			}
		}

		[Fact]
		public async Task DeleteApiScopeAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
			{
                var apiResourceService = GetApiResourceService(context);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

				await apiResourceService.AddApiResourceAsync(apiResourceDto);

				//Get new api resource
				var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

				var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

				//Assert new api resource
				apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

				//Generate random new api scope
				var apiScopeDtoMock = ApiResourceDtoMock.GenerateRandomApiScope(0, newApiResourceDto.Id);

				//Add new api scope
				await apiResourceService.AddApiScopeAsync(newApiResourceDto.Id, apiScopeDtoMock);

				//Get inserted api scope
				var apiScope = await context.ApiScopes.FirstOrDefaultAsync(x => x.Name == apiScopeDtoMock.Name && x.Id == newApiResourceDto.Id);

				//Map entity to model
				var apiScopesDto = apiScope.ToModel();

				//Get new api scope
				var newApiScope = await apiResourceService.GetApiScopeAsync(newApiResourceDto.Id, apiScopesDto.ApiScopeId);

				//Assert
				newApiScope.Should().BeEquivalentTo(apiScopesDto, o =>
					o.Excluding(x => x.ShowInDiscoveryDocument)
					 .Excluding(x => x.ApiResourceId)
					 .Excluding(x => x.ResourceName)
					 .Excluding(x => x.Description)
					 .Excluding(x => x.DisplayName)
					 .Excluding(x => x.PageSize)
					 .Excluding(x => x.TotalCount)
					 .Excluding(x => x.ApiScopeId)
					 .Excluding(x => x.Required)
					 .Excluding(x => x.Emphasize)
					 .Excluding(x => x.Scopes)
					 .Excluding(x => x.Name)
					 .Excluding(x => x.UserClaims)
				);

				//Delete it
				await apiResourceService.DeleteApiScopeAsync(newApiScope);

				var deletedApiScope = await context.ApiScopes.FirstOrDefaultAsync(x => x.Name == apiScopeDtoMock.Name && x.Id == newApiResourceDto.Id);

				//Assert after deleting
				deletedApiScope.Should().BeNull();
			}
		}

		[Fact]
		public async Task AddApiSecretAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
			{
                var apiResourceService = GetApiResourceService(context);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

				await apiResourceService.AddApiResourceAsync(apiResourceDto);

				//Get new api resource
				var apiResource = await context.ApiResources.SingleOrDefaultAsync(x => x.Name == apiResourceDto.Name);

				var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

				//Assert new api resource
				apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

				//Generate random new api secret
				var apiSecretsDto = ApiResourceDtoMock.GenerateRandomApiResourceSecret(0, newApiResourceDto.Id);

				//Add new api secret
				await apiResourceService.AddApiResourceSecretAsync(apiSecretsDto);

				//Get inserted api secret
				var apiSecret = await context.ApiResourceSecrets.FirstOrDefaultAsync(x => x.Value == apiSecretsDto.Value && x.ApiResource.Id == newApiResourceDto.Id);

				//Map entity to model
				var secretsDto = apiSecret.ToModel();

				//Get new api secret    
				var newApiSecret = await apiResourceService.GetApiSecretAsync(secretsDto.ApiSecretId);

				//Assert
				newApiSecret.Should().BeEquivalentTo(secretsDto, o => 
					o.Excluding(x => x.ApiResourceName)
					 .Excluding(o => o.ApiResourceSecrets));
			}
		}

		[Fact]
		public async Task DeleteApiSecretAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
			{
                var apiResourceService = GetApiResourceService(context);

                //Generate random new api resource
                var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(0);

				await apiResourceService.AddApiResourceAsync(apiResourceDto);

				//Get new api resource
				var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

				var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

				//Assert new api resource
				apiResourceDto.Should().BeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

				//Generate random new api secret
				var apiSecretsDtoMock = ApiResourceDtoMock.GenerateRandomApiResourceSecret(0, newApiResourceDto.Id);

				//Add new api secret
				await apiResourceService.AddApiResourceSecretAsync(apiSecretsDtoMock);

				//Get inserted api secret
				var apiResourceSecret = await context.ApiResourceSecrets.FirstOrDefaultAsync(x => x.Value == apiSecretsDtoMock.Value && x.ApiResource.Id == newApiResourceDto.Id);

				//Map entity to model
				var apiResourceSecretDto = apiResourceSecret.ToModel();

				//Get new api secret    
				var newApiResourceSecret = await apiResourceService.GetApiSecretAsync(apiResourceSecretDto.ApiResourceId);

				//Assert
				newApiResourceSecret.Should().BeEquivalentTo(apiResourceSecretDto, o => 
					o.Excluding(x => x.Description)
					 .Excluding(o => o.ApiResourceSecrets));

				//Delete it
				await apiResourceService.DeleteApiResourceSecretAsync(newApiResourceSecret);

				var deletedApiSecret = await context.ApiResourceSecrets.FirstOrDefaultAsync(x => x.Value == apiSecretsDtoMock.Value && x.ApiResource.Id == newApiResourceDto.Id);

				//Assert after deleting
				deletedApiSecret.Should().BeNull();
			}
		}

		[Fact]
		public async Task AddApiResourcePropertyAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
			{
                var apiResourceService = GetApiResourceService(context);

                //Generate random new api resource
                var apiResource = ApiResourceDtoMock.GenerateRandomApiResource(0);

				await apiResourceService.AddApiResourceAsync(apiResource);

				//Get new api resource
				var resource = await context.ApiResources.SingleOrDefaultAsync(x => x.Name == apiResource.Name);

				var apiResourceDto = await apiResourceService.GetApiResourceAsync(resource.Id);

				//Assert new api resource
				apiResource.Should().BeEquivalentTo(apiResourceDto, options => options.Excluding(o => o.Id));

				//Generate random new api resource property
				var apiResourceProperty = ApiResourceDtoMock.GenerateRandomApiResourceProperty(0, resource.Id);

				//Add new api resource property
				await apiResourceService.AddApiResourcePropertyAsync(apiResourceProperty);

				//Get inserted api resource property
				var property = await context.ApiResourceProperties.SingleOrDefaultAsync(x => x.Value == apiResourceProperty.Value && x.ApiResource.Id == resource.Id);

				//Map entity to model
				var propertyDto = property.ToModel();

				//Get new api resource property    
				var resourcePropertiesDto = await apiResourceService.GetApiResourcePropertyAsync(property.Id);

				//Assert
				resourcePropertiesDto.Should().BeEquivalentTo(propertyDto, options =>
					options.Excluding(o => o.ApiResourcePropertyId)
						   .Excluding(o => o.ApiResourceName));
			}
		}

		[Fact]
		public async Task GetApiResourcePropertyAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
			{
                var apiResourceService = GetApiResourceService(context);

                //Generate random new api resource
                var apiResource = ApiResourceDtoMock.GenerateRandomApiResource(0);

				await apiResourceService.AddApiResourceAsync(apiResource);

				//Get new api resource
				var resource = await context.ApiResources.FirstOrDefaultAsync(x => x.Name == apiResource.Name);

				var apiResourceDto = await apiResourceService.GetApiResourceAsync(resource.Id);

				//Assert new api resource
				apiResource.Should().BeEquivalentTo(apiResourceDto, options => options.Excluding(o => o.Id));

				//Generate random new api resource property
				var apiResourceProperty = ApiResourceDtoMock.GenerateRandomApiResourceProperty(0, resource.Id);

				//Add new api resource property
				await apiResourceService.AddApiResourcePropertyAsync(apiResourceProperty);

				//Get inserted api resource property
				var property = await context.ApiResourceProperties.FirstOrDefaultAsync(x => x.Value == apiResourceProperty.Value && x.ApiResource.Id == resource.Id);

				//Map entity to model
				var propertyDto = property.ToModel();

				//Get new api resource property    
				var apiResourcePropertiesDto = await apiResourceService.GetApiResourcePropertyAsync(property.Id);

				//Assert
				apiResourcePropertiesDto.Should().BeEquivalentTo(propertyDto, options =>
					options.Excluding(o => o.ApiResourcePropertyId)
					.Excluding(o => o.ApiResourceName));
			}
		}

		[Fact]
		public async Task DeleteApiResourcePropertyAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
			{
                var apiResourceService = GetApiResourceService(context);

                //Generate random new api resource
                var apiResource = ApiResourceDtoMock.GenerateRandomApiResource(0);

				await apiResourceService.AddApiResourceAsync(apiResource);

				//Get new api resource
				var resource = await context.ApiResources.FirstOrDefaultAsync(x => x.Name == apiResource.Name);

				var apiResourceDto = await apiResourceService.GetApiResourceAsync(resource.Id);

				//Assert new api resource
				apiResource.Should().BeEquivalentTo(apiResourceDto, options => options.Excluding(o => o.Id));

				//Generate random new api resource Property
				var apiResourcePropertiesDto = ApiResourceDtoMock.GenerateRandomApiResourceProperty(0, resource.Id);

				//Add new api resource Property
				await apiResourceService.AddApiResourcePropertyAsync(apiResourcePropertiesDto);

				//Get inserted api resource Property
				var property = await context.ApiResourceProperties.FirstOrDefaultAsync(x => x.Value == apiResourcePropertiesDto.Value && x.ApiResource.Id == resource.Id);

				//Map entity to model
				var propertiesDto = property.ToModel();

				//Get new api resource Property    
				var resourcePropertiesDto = await apiResourceService.GetApiResourcePropertyAsync(property.Id);

				//Assert
				resourcePropertiesDto.Should().BeEquivalentTo(propertiesDto, options => 
					options.Excluding(o => o.ApiResourcePropertyId)
					.Excluding(o => o.ApiResourceName));

				//Delete api resource Property
				await apiResourceService.DeleteApiResourcePropertyAsync(resourcePropertiesDto);

				//Get removed api resource Property
				var apiResourceProperty = await context.ApiResourceProperties.FirstOrDefaultAsync(x => x.Id == property.Id);

				//Assert after delete it
				apiResourceProperty.Should().BeNull();
			}
		}
	}
}