using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Repositories
{
    public class ApiResourceRepositoryTests
    {
        private readonly DbContextOptions<IdentityServerConfigurationDbContext> _dbContextOptions;
        private readonly ConfigurationStoreOptions _storeOptions;
        private readonly OperationalStoreOptions _operationalStore;

        public ApiResourceRepositoryTests()
        {
            var databaseName = Guid.NewGuid().ToString();

            _dbContextOptions = new DbContextOptionsBuilder<IdentityServerConfigurationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            _storeOptions = new ConfigurationStoreOptions();
            _operationalStore = new OperationalStoreOptions();
        }

        private IApiResourceRepository GetApiResourceRepository(IdentityServerConfigurationDbContext context)
        {
            IApiResourceRepository apiResourceRepository = new ApiResourceRepository<IdentityServerConfigurationDbContext>(context);

            return apiResourceRepository;
        }


        [Fact]
        public async Task AddApiResourceAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiResourceRepository(context);

                //Generate random new api resource
                var apiResource = ApiResourceMock.GenerateRandomApiResource(0);

                //Add new api resource
                await apiResourceRepository.AddApiResourceAsync(apiResource);

                //Get new api resource
                var newApiResource = await context.ApiResources.Where(x => x.Id == apiResource.Id).SingleAsync();

                //Assert new api resource
                newApiResource.Should().BeEquivalentTo(apiResource, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task GetApiResourceAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiResourceRepository(context);

                //Generate random new api resource
                var apiResource = ApiResourceMock.GenerateRandomApiResource(0);

                //Add new api resource
                await apiResourceRepository.AddApiResourceAsync(apiResource);

                //Get new api resource
                var newApiResource = await apiResourceRepository.GetApiResourceAsync(apiResource.Id);

                //Assert new api resource
                newApiResource.Should().BeEquivalentTo(apiResource, options => 
                    options.Excluding(o => o.Id)
                           .Excluding(o => o.Secrets)
                           .Excluding(o => o.Scopes)
                           .Excluding(o => o.Properties)
                           .Excluding(o => o.UserClaims));

                newApiResource.UserClaims.Should().BeEquivalentTo(apiResource.UserClaims, option => 
                    option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                          .Excluding(x => x.SelectedMemberPath.EndsWith("ApiResource")));
            }
        }

        [Fact]
        public async Task DeleteApiResourceAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiResourceRepository(context);

                //Generate random new api resource
                var apiResource = ApiResourceMock.GenerateRandomApiResource(0);

                //Add new api resource
                await apiResourceRepository.AddApiResourceAsync(apiResource);

                //Get new api resource
                var newApiResource = await context.ApiResources.SingleAsync(x => x.Id == apiResource.Id);

                //Assert new api resource
                newApiResource.Should().BeEquivalentTo(apiResource, options => options.Excluding(o => o.Id));

                //Delete api resource
                await apiResourceRepository.DeleteApiResourceAsync(newApiResource);

                //Get deleted api resource
                var deletedApiResource = await context.ApiResources.SingleOrDefaultAsync(x => x.Id == apiResource.Id);

                //Assert if it not exist
                deletedApiResource.Should().BeNull();
            }
        }

        [Fact]
        public async Task UpdateApiResourceAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiResourceRepository(context);

                //Generate random new api resource
                var apiResource = ApiResourceMock.GenerateRandomApiResource(0);

                //Add new api resource
                await apiResourceRepository.AddApiResourceAsync(apiResource);

                //Get new api resource
                var newApiResource = await context.ApiResources.SingleOrDefaultAsync(x => x.Id == apiResource.Id);

                //Assert new api resource
                newApiResource.Should().BeEquivalentTo(apiResource, options => options.Excluding(o => o.Id));

                //Detached the added item
                context.Entry(newApiResource).State = EntityState.Detached;

                //Generete new api resource with added item id
                var updatedApiResource = ApiResourceMock.GenerateRandomApiResource(newApiResource.Id);

                //Update api resource
                await apiResourceRepository.UpdateApiResourceAsync(updatedApiResource);

                //Get updated api resource
                var updatedApiResourceEntity = await context.ApiResources.SingleAsync(x => x.Id == updatedApiResource.Id);

                //Assert updated api resource
                updatedApiResource.Should().BeEquivalentTo(updatedApiResourceEntity);
            }
        }

        [Fact]
        public async Task AddApiScopeAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiResourceRepository(context);

                //Generate random new api resource
                var apiResource = ApiResourceMock.GenerateRandomApiResource(0);

                //Add new api resource
                await apiResourceRepository.AddApiResourceAsync(apiResource);

                //Generate random new api scope
                var apiScope = ApiResourceMock.GenerateRandomApiScope(0);

                //Add new api scope
                await apiResourceRepository.AddApiScopeAsync(apiResource.Id, apiScope);

                //Get new api scope
                var newApiScopes = await context.ApiScopes.Where(x => x.Id == apiScope.Id).SingleAsync();

                //Assert new api scope
                newApiScopes.Should().BeEquivalentTo(apiScope, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task UpdateApiScopeAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiResourceRepository(context);

                //Generate random new api resource
                var apiResource = ApiResourceMock.GenerateRandomApiResource(0);

                //Add new api resource
                await apiResourceRepository.AddApiResourceAsync(apiResource);

                //Get new api resource
                var newApiResource = await context.ApiResources.Where(x => x.Id == apiResource.Id).SingleOrDefaultAsync();

                //Assert new api resource
                newApiResource.Should().BeEquivalentTo(apiResource, options => options.Excluding(o => o.Id));

                //Detached the added item
                context.Entry(newApiResource).State = EntityState.Detached;

                //Generate random new api scope
                var apiScope = ApiResourceMock.GenerateRandomApiScope(0);

                //Add new api scope
                await apiResourceRepository.AddApiScopeAsync(apiResource.Id, apiScope);

                //Detached the added item
                context.Entry(apiScope).State = EntityState.Detached;

                //Generete new api scope with added item id
                var updatedApiScope = ApiResourceMock.GenerateRandomApiScope(apiScope.Id);

                //Update api scope
                await apiResourceRepository.UpdateApiScopeAsync(apiResource.Id, updatedApiScope);

                //Get updated api scope
                var updatedApiScopeEntity = await context.ApiScopes.Where(x => x.Id == updatedApiScope.Id).SingleAsync();

                //Assert updated api scope
                updatedApiScope.Should().BeEquivalentTo(updatedApiScopeEntity);
            }
        }

        [Fact]
        public async Task DeleteApiScopeAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiResourceRepository(context);

                //Generate random new api resource
                var apiResource = ApiResourceMock.GenerateRandomApiResource(0);

                //Add new api resource
                await apiResourceRepository.AddApiResourceAsync(apiResource);

                //Generate random new api scope
                var apiScope = ApiResourceMock.GenerateRandomApiScope(0);

                //Add new api resource
                await apiResourceRepository.AddApiScopeAsync(apiResource.Id, apiScope);

                //Get new api resource
                var newApiScopes = await context.ApiScopes.Where(x => x.Id == apiScope.Id).SingleOrDefaultAsync();

                //Assert new api resource
                newApiScopes.Should().BeEquivalentTo(apiScope, options => options.Excluding(o => o.Id));

                //Try delete it
                await apiResourceRepository.DeleteApiScopeAsync(newApiScopes);

                //Get new api scope
                var deletedApiScopes = await context.ApiScopes.Where(x => x.Id == newApiScopes.Id).SingleOrDefaultAsync();

                //Assert if it exist
                deletedApiScopes.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetApiScopeAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiResourceRepository(context);

                //Generate random new api resource
                var apiResource = ApiResourceMock.GenerateRandomApiResource(0);

                //Add new api resource
                await apiResourceRepository.AddApiResourceAsync(apiResource);

                //Generate random new api scope
                var apiScope = ApiResourceMock.GenerateRandomApiScope(0);

                //Add new api scope
                await apiResourceRepository.AddApiScopeAsync(apiResource.Id, apiScope);

                //Get new api scope
                var newApiScopes = await apiResourceRepository.GetApiScopeAsync(apiScope.Id);

                //Assert new api resource
                newApiScopes.Should().BeEquivalentTo(apiScope, options => 
                    options.Excluding(o => o.Id)
                           .Excluding(o => o.Properties)
                           .Excluding(o => o.UserClaims));

                newApiScopes.UserClaims.Should().BeEquivalentTo(apiScope.UserClaims,option => 
                    option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                          .Excluding(x => x.SelectedMemberPath.EndsWith("Scope")));
            }
        }

        [Fact]
        public async Task AddApiSecretAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiResourceRepository(context);

                //Generate random new api resource
                var apiResource = ApiResourceMock.GenerateRandomApiResource(0);

                //Add new api resource
                await apiResourceRepository.AddApiResourceAsync(apiResource);

                //Generate random new api secret
                var apiSecret = ApiResourceMock.GenerateRandomApiSecret(0);

                //Add new api secret
                await apiResourceRepository.AddApiResourceSecretAsync(apiResource.Id, apiSecret);

                //Get new api secret
                var newApiSecret = await context.ApiResourceSecrets.Where(x => x.Id == apiSecret.Id).SingleAsync();

                //Assert new api secret
                newApiSecret.Should().BeEquivalentTo(apiSecret, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task DeleteApiSecretAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiResourceRepository(context);

                //Generate random new api resource
                var apiResource = ApiResourceMock.GenerateRandomApiResource(0);

                //Add new api resource
                await apiResourceRepository.AddApiResourceAsync(apiResource);

                //Generate random new api scope
                var apiSecret = ApiResourceMock.GenerateRandomApiSecret(0);

                //Add new api secret
                await apiResourceRepository.AddApiResourceSecretAsync(apiResource.Id, apiSecret);

                //Get new api resource
                var newApiSecret = await context.ApiResourceSecrets.Where(x => x.Id == apiSecret.Id).SingleOrDefaultAsync();

                //Assert new api resource
                newApiSecret.Should().BeEquivalentTo(apiSecret, options => options.Excluding(o => o.Id));

                //Try delete it
                await apiResourceRepository.DeleteApiResourceSecretAsync(newApiSecret);

                //Get deleted api secret
                var deletedApiSecret = await context.ApiResourceSecrets.Where(x => x.Id == newApiSecret.Id).SingleOrDefaultAsync();

                //Assert if it exist
                deletedApiSecret.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetApiSecretAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiResourceRepository(context);

                //Generate random new api resource
                var apiResource = ApiResourceMock.GenerateRandomApiResource(0);

                //Add new api resource
                await apiResourceRepository.AddApiResourceAsync(apiResource);

                //Generate random new api secret
                var apiSecret = ApiResourceMock.GenerateRandomApiSecret(0);

                //Add new api secret
                await apiResourceRepository.AddApiResourceSecretAsync(apiResource.Id, apiSecret);

                //Get new api secret
                var newApiSecret = await apiResourceRepository.GetApiResourceSecretAsync(apiSecret.Id);

                //Assert new api secret
                newApiSecret.Should().BeEquivalentTo(apiSecret, options => 
                    options.Excluding(o => o.Id)
                           .Excluding(o => o.ApiResource.Secrets)
                           .Excluding(o => o.ApiResource.UserClaims)
                           .Excluding(o => o.ApiResource.Properties)
                           .Excluding(o => o.ApiResource.Scopes));
            }
        }

        [Fact]
        public async Task AddApiResourcePropertyAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiResourceRepository(context);

                //Generate random new api resource without id
                var apiResource = ApiResourceMock.GenerateRandomApiResource(0);

                //Add new api resource
                await apiResourceRepository.AddApiResourceAsync(apiResource);

                //Get new api resource
                var resource = await apiResourceRepository.GetApiResourceAsync(apiResource.Id);

                //Assert new api resource
                resource.Should().BeEquivalentTo(apiResource, options => 
                    options.Excluding(o => o.Id)
                           .Excluding(o => o.Secrets)
                           .Excluding(o => o.Scopes)
                           .Excluding(o => o.Properties)
                           .Excluding(o => o.UserClaims));

                resource.UserClaims.Should().BeEquivalentTo(apiResource.UserClaims,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                                    .Excluding(x => x.SelectedMemberPath.EndsWith("ApiResource")));

                //Generate random new api resource property
                var apiResourceProperty = ApiResourceMock.GenerateRandomApiResourceProperty(0);

                //Add new api resource property
                await apiResourceRepository.AddApiResourcePropertyAsync(resource.Id, apiResourceProperty);

                //Get new api resource property
                var resourceProperty = await context.ApiResourceProperties.SingleOrDefaultAsync(x => x.Id == apiResourceProperty.Id);

                resourceProperty.Should().BeEquivalentTo(apiResourceProperty, options => 
                    options.Excluding(o => o.Id)
                           .Excluding(x => x.ApiResource));
            }
        }

        [Fact]
        public async Task DeleteApiResourcePropertyAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiResourceRepository(context);

                //Generate random new api resource without id
                var apiResource = ApiResourceMock.GenerateRandomApiResource(0);

                //Add new api resource
                await apiResourceRepository.AddApiResourceAsync(apiResource);

                //Get new api resource
                var resource = await apiResourceRepository.GetApiResourceAsync(apiResource.Id);

                //Assert new api resource
                resource.Should().BeEquivalentTo(apiResource, options => 
                    options.Excluding(o => o.Id)
                           .Excluding(o => o.Secrets)
                           .Excluding(o => o.Scopes)
                           .Excluding(o => o.Properties)
                           .Excluding(o => o.UserClaims));

                resource.UserClaims.Should().BeEquivalentTo(apiResource.UserClaims, option => 
                    option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                          .Excluding(x => x.SelectedMemberPath.EndsWith("ApiResource")));

                //Generate random new api resource property
                var apiResourceProperty = ApiResourceMock.GenerateRandomApiResourceProperty(0);

                //Add new api resource property
                await apiResourceRepository.AddApiResourcePropertyAsync(resource.Id, apiResourceProperty);

                //Get new api resource property
                var property = await context.ApiResourceProperties.SingleOrDefaultAsync(x => x.Id == apiResourceProperty.Id);

                //Assert
                property.Should().BeEquivalentTo(apiResourceProperty,
                    options => options.Excluding(o => o.Id).Excluding(x => x.ApiResource));

                //Try delete it
                await apiResourceRepository.DeleteApiResourcePropertyAsync(property);

                //Get new api resource property
                var resourceProperty = await context.ApiResourceProperties.SingleOrDefaultAsync(x => x.Id == apiResourceProperty.Id);

                //Assert
                resourceProperty.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetApiResourcePropertyAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiResourceRepository(context);

                //Generate random new api resource without id
                var apiResource = ApiResourceMock.GenerateRandomApiResource(0);

                //Add new api resource
                await apiResourceRepository.AddApiResourceAsync(apiResource);

                //Get new api resource
                var resource = await apiResourceRepository.GetApiResourceAsync(apiResource.Id);

                //Assert new api resource
                resource.Should().BeEquivalentTo(apiResource, options => 
                    options.Excluding(o => o.Id)
                           .Excluding(o => o.Secrets)
                           .Excluding(o => o.Scopes)
                           .Excluding(o => o.Properties)
                           .Excluding(o => o.UserClaims));

                resource.UserClaims.Should().BeEquivalentTo(apiResource.UserClaims, option => 
                    option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                          .Excluding(x => x.SelectedMemberPath.EndsWith("ApiResource")));

                //Generate random new api resource property
                var apiResourceProperty = ApiResourceMock.GenerateRandomApiResourceProperty(0);

                //Add new api resource property
                await apiResourceRepository.AddApiResourcePropertyAsync(resource.Id, apiResourceProperty);

                //Get new api resource property
                var resourceProperty = await apiResourceRepository.GetApiResourcePropertyAsync(apiResourceProperty.Id);

                resourceProperty.Should().BeEquivalentTo(apiResourceProperty, options => 
                    options.Excluding(o => o.Id)
                           .Excluding(x => x.ApiResource));
            }
        }
    }
}