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
    public class ApiScopeRepositoryTests
    {
        private readonly DbContextOptions<IdentityServerConfigurationDbContext> _dbContextOptions;
        private readonly ConfigurationStoreOptions _storeOptions;

        public ApiScopeRepositoryTests()
        {
            var databaseName = Guid.NewGuid().ToString();

            _dbContextOptions = new DbContextOptionsBuilder<IdentityServerConfigurationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            _storeOptions = new ConfigurationStoreOptions();
        }

        private IApiScopeRepository GetApiScopeRepository(IdentityServerConfigurationDbContext context)
        {
            IApiScopeRepository apiScopeRepository = new ApiScopeRepository<IdentityServerConfigurationDbContext>(context);

            return apiScopeRepository;
        }

        [Fact]
        public async Task AddApiScopeAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiScopeRepository(context);

                //Generate random new api scope
                var apiScope = ApiScopeMock.GenerateRandomApiScope(0);

                //Add new api scope
                await apiResourceRepository.AddApiScopeAsync(apiScope);

                //Get new api scope
                var newApiScopes = await context.ApiScopes.Where(x => x.Id == apiScope.Id).SingleAsync();

                //Assert new api scope
                newApiScopes.ShouldBeEquivalentTo(apiScope, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task UpdateApiScopeAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiScopeRepository(context);

                //Generate random new api scope
                var apiScope = ApiScopeMock.GenerateRandomApiScope(0);

                //Add new api scope
                await apiResourceRepository.AddApiScopeAsync(apiScope);

                //Detached the added item
                context.Entry(apiScope).State = EntityState.Detached;

                //Generete new api scope with added item id
                var updatedApiScope = ApiScopeMock.GenerateRandomApiScope(apiScope.Id);

                //Update api scope
                await apiResourceRepository.UpdateApiScopeAsync(updatedApiScope);

                //Get updated api scope
                var updatedApiScopeEntity = await context.ApiScopes.Where(x => x.Id == updatedApiScope.Id).SingleAsync();

                //Assert updated api scope
                updatedApiScope.ShouldBeEquivalentTo(updatedApiScopeEntity);
            }
        }

        [Fact]
        public async Task DeleteApiScopeAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var apiResourceRepository = GetApiScopeRepository(context);

                //Generate random new api scope
                var apiScope = ApiScopeMock.GenerateRandomApiScope(0);

                //Add new api resource
                await apiResourceRepository.AddApiScopeAsync(apiScope);

                //Get new api resource
                var newApiScopes = await context.ApiScopes.Where(x => x.Id == apiScope.Id).SingleOrDefaultAsync();

                //Assert new api resource
                newApiScopes.ShouldBeEquivalentTo(apiScope, options => options.Excluding(o => o.Id));

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
                var apiResourceRepository = GetApiScopeRepository(context);

                //Generate random new api scope
                var apiScope = ApiScopeMock.GenerateRandomApiScope(0);

                //Add new api scope
                await apiResourceRepository.AddApiScopeAsync(apiScope);

                //Get new api scope
                var newApiScopes = await apiResourceRepository.GetApiScopeAsync(apiScope.Id);

                //Assert new api resource
                newApiScopes.ShouldBeEquivalentTo(apiScope, options => options.Excluding(o => o.Id)
                    .Excluding(o => o.UserClaims));

                newApiScopes.UserClaims.ShouldBeEquivalentTo(apiScope.UserClaims,
                    option => option.Excluding(x => x.SelectedMemberPath.EndsWith("Id"))
                        .Excluding(x => x.SelectedMemberPath.EndsWith("Scope")));
            }
        }
    }
}
