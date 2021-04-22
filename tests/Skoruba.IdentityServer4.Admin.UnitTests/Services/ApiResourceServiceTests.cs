﻿using System;
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
				apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));
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
				apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));
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
				apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

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
				apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

				//Detached the added item
				context.Entry(apiResource).State = EntityState.Detached;

				//Generete new api resuorce with added item id
				var updatedApiResource = ApiResourceDtoMock.GenerateRandomApiResource(apiResource.Id);

				//Update api resource
				await apiResourceService.UpdateApiResourceAsync(updatedApiResource);

				var updatedApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

				//Assert updated api resuorce
				updatedApiResource.ShouldBeEquivalentTo(updatedApiResourceDto, options => options.Excluding(o => o.Id));
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
				var apiResource = await context.ApiResources.Where(x => x.Name == apiResourceDto.Name).SingleOrDefaultAsync();

				var newApiResourceDto = await apiResourceService.GetApiResourceAsync(apiResource.Id);

				//Assert new api resource
				apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

				//Generate random new api secret
				var apiSecretsDto = ApiResourceDtoMock.GenerateRandomApiSecret(0, newApiResourceDto.Id);

				//Add new api secret
				await apiResourceService.AddApiSecretAsync(apiSecretsDto);

				//Get inserted api secret
				var apiSecret = await context.ApiSecrets.Where(x => x.Value == apiSecretsDto.Value && x.ApiResource.Id == newApiResourceDto.Id)
					.SingleOrDefaultAsync();

				//Map entity to model
				var secretsDto = apiSecret.ToModel();

				//Get new api secret    
				var newApiSecret = await apiResourceService.GetApiSecretAsync(secretsDto.ApiSecretId);

                //Assert secret value
                secretsDto.Value.Should().Be(apiSecretsDto.Value);

                //Assert
                newApiSecret.ShouldBeEquivalentTo(secretsDto, o => o.Excluding(x => x.ApiResourceName).Excluding(x => x.Value));
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
				apiResourceDto.ShouldBeEquivalentTo(newApiResourceDto, options => options.Excluding(o => o.Id));

				//Generate random new api secret
				var apiSecretsDtoMock = ApiResourceDtoMock.GenerateRandomApiSecret(0, newApiResourceDto.Id);

				//Add new api secret
				await apiResourceService.AddApiSecretAsync(apiSecretsDtoMock);

				//Get inserted api secret
				var apiSecret = await context.ApiSecrets.Where(x => x.Value == apiSecretsDtoMock.Value && x.ApiResource.Id == newApiResourceDto.Id)
					.SingleOrDefaultAsync();

				//Map entity to model
				var apiSecretsDto = apiSecret.ToModel();

				//Get new api secret    
				var newApiSecret = await apiResourceService.GetApiSecretAsync(apiSecretsDto.ApiSecretId);

                // Assert
                newApiSecret.ShouldBeEquivalentTo(apiSecretsDto, o => o.Excluding(x => x.ApiResourceName).Excluding(x => x.Value));

                apiSecretsDto.Value.Should().Be(apiSecretsDtoMock.Value);

				//Delete it
				await apiResourceService.DeleteApiSecretAsync(newApiSecret);

				var deletedApiSecret = await context.ApiSecrets.Where(x => x.Value == apiSecretsDtoMock.Value && x.ApiResource.Id == newApiResourceDto.Id)
					.SingleOrDefaultAsync();

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
				var resource = await context.ApiResources.Where(x => x.Name == apiResource.Name).SingleOrDefaultAsync();

				var apiResourceDto = await apiResourceService.GetApiResourceAsync(resource.Id);

				//Assert new api resource
				apiResource.ShouldBeEquivalentTo(apiResourceDto, options => options.Excluding(o => o.Id));

				//Generate random new api resource property
				var apiResourceProperty = ApiResourceDtoMock.GenerateRandomApiResourceProperty(0, resource.Id);

				//Add new api resource property
				await apiResourceService.AddApiResourcePropertyAsync(apiResourceProperty);

				//Get inserted api resource property
				var property = await context.ApiResourceProperties.Where(x => x.Value == apiResourceProperty.Value && x.ApiResource.Id == resource.Id)
					.SingleOrDefaultAsync();

				//Map entity to model
				var propertyDto = property.ToModel();

				//Get new api resource property    
				var resourcePropertiesDto = await apiResourceService.GetApiResourcePropertyAsync(property.Id);

				//Assert
				resourcePropertiesDto.ShouldBeEquivalentTo(propertyDto, options =>
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
				var resource = await context.ApiResources.Where(x => x.Name == apiResource.Name).SingleOrDefaultAsync();

				var apiResourceDto = await apiResourceService.GetApiResourceAsync(resource.Id);

				//Assert new api resource
				apiResource.ShouldBeEquivalentTo(apiResourceDto, options => options.Excluding(o => o.Id));

				//Generate random new api resource property
				var apiResourceProperty = ApiResourceDtoMock.GenerateRandomApiResourceProperty(0, resource.Id);

				//Add new api resource property
				await apiResourceService.AddApiResourcePropertyAsync(apiResourceProperty);

				//Get inserted api resource property
				var property = await context.ApiResourceProperties.Where(x => x.Value == apiResourceProperty.Value && x.ApiResource.Id == resource.Id)
					.SingleOrDefaultAsync();

				//Map entity to model
				var propertyDto = property.ToModel();

				//Get new api resource property    
				var apiResourcePropertiesDto = await apiResourceService.GetApiResourcePropertyAsync(property.Id);

				//Assert
				apiResourcePropertiesDto.ShouldBeEquivalentTo(propertyDto, options =>
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
				var resource = await context.ApiResources.Where(x => x.Name == apiResource.Name).SingleOrDefaultAsync();

				var apiResourceDto = await apiResourceService.GetApiResourceAsync(resource.Id);

				//Assert new api resource
				apiResource.ShouldBeEquivalentTo(apiResourceDto, options => options.Excluding(o => o.Id));

				//Generate random new api resource Property
				var apiResourcePropertiesDto = ApiResourceDtoMock.GenerateRandomApiResourceProperty(0, resource.Id);

				//Add new api resource Property
				await apiResourceService.AddApiResourcePropertyAsync(apiResourcePropertiesDto);

				//Get inserted api resource Property
				var property = await context.ApiResourceProperties.Where(x => x.Value == apiResourcePropertiesDto.Value && x.ApiResource.Id == resource.Id)
					.SingleOrDefaultAsync();

				//Map entity to model
				var propertiesDto = property.ToModel();

				//Get new api resource Property    
				var resourcePropertiesDto = await apiResourceService.GetApiResourcePropertyAsync(property.Id);

				//Assert
				resourcePropertiesDto.ShouldBeEquivalentTo(propertiesDto, options => 
					options.Excluding(o => o.ApiResourcePropertyId)
					.Excluding(o => o.ApiResourceName));

				//Delete api resource Property
				await apiResourceService.DeleteApiResourcePropertyAsync(resourcePropertiesDto);

				//Get removed api resource Property
				var apiResourceProperty = await context.ApiResourceProperties.Where(x => x.Id == property.Id).SingleOrDefaultAsync();

				//Assert after delete it
				apiResourceProperty.Should().BeNull();
			}
		}
	}
}