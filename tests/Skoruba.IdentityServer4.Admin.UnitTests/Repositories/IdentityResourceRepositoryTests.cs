using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Repositories
{
    public class IdentityResourceRepositoryTests
    {
        private readonly DbContextOptions<IdentityServerConfigurationDbContext> _dbContextOptions;
        private readonly ConfigurationStoreOptions _storeOptions;
        private readonly OperationalStoreOptions _operationalStore;

        private IIdentityResourceRepository GetIdentityResourceRepository(IdentityServerConfigurationDbContext context)
        {
            IIdentityResourceRepository identityResourceRepository = new IdentityResourceRepository<IdentityServerConfigurationDbContext>(context);

            return identityResourceRepository;
        }

        public IdentityResourceRepositoryTests()
        {
            var databaseName = Guid.NewGuid().ToString();

            _dbContextOptions = new DbContextOptionsBuilder<IdentityServerConfigurationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            _storeOptions = new ConfigurationStoreOptions();
            _operationalStore = new OperationalStoreOptions();
        }

        [Fact]
        public async Task AddIdentityResourceAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var identityResourceRepository = GetIdentityResourceRepository(context);

                //Generate random new identity resource
                var identityResource = IdentityResourceMock.GenerateRandomIdentityResource(0);

                //Add new identity resource
                await identityResourceRepository.AddIdentityResourceAsync(identityResource);

                //Get new identity resource
                var newIdentityResource = await context.IdentityResources.Where(x => x.Id == identityResource.Id).SingleAsync();

                //Assert new identity resource
                newIdentityResource.ShouldBeEquivalentTo(identityResource, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task GetIdentityResourceAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var identityResourceRepository = GetIdentityResourceRepository(context);

                //Generate random new identity resource
                var identityResource = IdentityResourceMock.GenerateRandomIdentityResource(0);

                //Add new identity resource
                await identityResourceRepository.AddIdentityResourceAsync(identityResource);

                //Get new identity resource
                var newIdentityResource = await identityResourceRepository.GetIdentityResourceAsync(identityResource.Id);

                //Assert new identity resource
                newIdentityResource.ShouldBeEquivalentTo(identityResource, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task DeleteIdentityResourceAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var identityResourceRepository = GetIdentityResourceRepository(context);

                //Generate random new identity resource
                var identityResource = IdentityResourceMock.GenerateRandomIdentityResource(0);

                //Add new identity resource
                await identityResourceRepository.AddIdentityResourceAsync(identityResource);

                //Get new identity resource
                var newIdentityResource = await context.IdentityResources.Where(x => x.Id == identityResource.Id).SingleAsync();

                //Assert new identity resource
                newIdentityResource.ShouldBeEquivalentTo(identityResource, options => options.Excluding(o => o.Id));

                //Delete identity resource
                await identityResourceRepository.DeleteIdentityResourceAsync(newIdentityResource);

                //Get deleted identity resource
                var deletedIdentityResource = await context.IdentityResources.Where(x => x.Id == identityResource.Id).SingleOrDefaultAsync();

                //Assert if it not exist
                deletedIdentityResource.Should().BeNull();
            }
        }

        [Fact]
        public async Task UpdateIdentityResourceAsync()
        {
            using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
            {
                var identityResourceRepository = GetIdentityResourceRepository(context);

                //Generate random new identity resource
                var identityResource = IdentityResourceMock.GenerateRandomIdentityResource(0);

                //Add new identity resource
                await identityResourceRepository.AddIdentityResourceAsync(identityResource);

                //Get new identity resource
                var newIdentityResource = await context.IdentityResources.Where(x => x.Id == identityResource.Id).SingleOrDefaultAsync();

                //Assert new identity resource
                newIdentityResource.ShouldBeEquivalentTo(identityResource, options => options.Excluding(o => o.Id));

                //Detached the added item
                context.Entry(newIdentityResource).State = EntityState.Detached;

                //Generete new identity resource with added item id
                var updatedIdentityResource = IdentityResourceMock.GenerateRandomIdentityResource(newIdentityResource.Id);

                //Update identity resource
                await identityResourceRepository.UpdateIdentityResourceAsync(updatedIdentityResource);

                //Get updated identity resource
                var updatedIdentityResourceEntity = await context.IdentityResources.Where(x => x.Id == updatedIdentityResource.Id).SingleAsync();

                //Assert updated identity resource
                updatedIdentityResource.ShouldBeEquivalentTo(updatedIdentityResourceEntity);
            }
        }

		[Fact]
		public async Task AddIdentityResourcePropertyAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
			{
				var identityResourceRepository = GetIdentityResourceRepository(context);

				//Generate random new identity resource without id
				var identityResource = IdentityResourceMock.GenerateRandomIdentityResource(0);

				//Add new identity resource
				await identityResourceRepository.AddIdentityResourceAsync(identityResource);

				//Get new identity resource
				var resource = await identityResourceRepository.GetIdentityResourceAsync(identityResource.Id);

				//Assert new identity resource
				resource.ShouldBeEquivalentTo(identityResource, options => options.Excluding(o => o.Id));

				//Generate random new identity resource property
				var identityResourceProperty = IdentityResourceMock.GenerateRandomIdentityResourceProperty(0);

				//Add new identity resource property
				await identityResourceRepository.AddIdentityResourcePropertyAsync(resource.Id, identityResourceProperty);

				//Get new identity resource property
				var resourceProperty = await context.IdentityResourceProperties.Where(x => x.Id == identityResourceProperty.Id)
					.SingleOrDefaultAsync();

				resourceProperty.ShouldBeEquivalentTo(identityResourceProperty,
					options => options.Excluding(o => o.Id).Excluding(x => x.IdentityResource));
			}
		}

		[Fact]
		public async Task DeleteIdentityResourcePropertyAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
			{
				var identityResourceRepository = GetIdentityResourceRepository(context);

				//Generate random new identity resource without id
				var identityResource = IdentityResourceMock.GenerateRandomIdentityResource(0);

				//Add new identity resource
				await identityResourceRepository.AddIdentityResourceAsync(identityResource);

				//Get new identity resource
				var resource = await identityResourceRepository.GetIdentityResourceAsync(identityResource.Id);

				//Assert new identity resource
				resource.ShouldBeEquivalentTo(identityResource, options => options.Excluding(o => o.Id));

				//Generate random new identity resource property
				var identityResourceProperty = IdentityResourceMock.GenerateRandomIdentityResourceProperty(0);

				//Add new identity resource property
				await identityResourceRepository.AddIdentityResourcePropertyAsync(resource.Id, identityResourceProperty);

				//Get new identity resource property
				var property = await context.IdentityResourceProperties.Where(x => x.Id == identityResourceProperty.Id)
					.SingleOrDefaultAsync();

				//Assert
				property.ShouldBeEquivalentTo(identityResourceProperty,
					options => options.Excluding(o => o.Id).Excluding(x => x.IdentityResource));

				//Try delete it
				await identityResourceRepository.DeleteIdentityResourcePropertyAsync(property);

				//Get new identity resource property
				var resourceProperty = await context.IdentityResourceProperties.Where(x => x.Id == identityResourceProperty.Id)
					.SingleOrDefaultAsync();

				//Assert
				resourceProperty.Should().BeNull();
			}
		}

		[Fact]
		public async Task GetIdentityResourcePropertyAsync()
		{
			using (var context = new IdentityServerConfigurationDbContext(_dbContextOptions, _storeOptions))
			{
				var identityResourceRepository = GetIdentityResourceRepository(context);

				//Generate random new identity resource without id
				var identityResource = IdentityResourceMock.GenerateRandomIdentityResource(0);

				//Add new identity resource
				await identityResourceRepository.AddIdentityResourceAsync(identityResource);

				//Get new identity resource
				var resource = await identityResourceRepository.GetIdentityResourceAsync(identityResource.Id);

				//Assert new identity resource
				resource.ShouldBeEquivalentTo(identityResource, options => options.Excluding(o => o.Id));

				//Generate random new identity resource property
				var identityResourceProperty = IdentityResourceMock.GenerateRandomIdentityResourceProperty(0);

				//Add new identity resource property
				await identityResourceRepository.AddIdentityResourcePropertyAsync(resource.Id, identityResourceProperty);

				//Get new identity resource property
				var resourceProperty = await identityResourceRepository.GetIdentityResourcePropertyAsync(identityResourceProperty.Id);

				resourceProperty.ShouldBeEquivalentTo(identityResourceProperty,
					options => options.Excluding(o => o.Id).Excluding(x => x.IdentityResource));
			}
		}
	}
}
