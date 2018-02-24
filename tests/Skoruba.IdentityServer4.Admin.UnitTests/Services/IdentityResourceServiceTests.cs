using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using Skoruba.IdentityServer4.Admin.Data.DbContexts;
using Skoruba.IdentityServer4.Admin.Data.Repositories;
using Skoruba.IdentityServer4.Admin.Services;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Services
{
    public class IdentityResourceServiceTests
    {
        public IdentityResourceServiceTests()
        {
            var databaseName = Guid.NewGuid().ToString();

            _dbContextOptions = new DbContextOptionsBuilder<AdminDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            _storeOptions = new ConfigurationStoreOptions();
            _operationalStore = new OperationalStoreOptions();
        }

        private readonly DbContextOptions<AdminDbContext> _dbContextOptions;
        private readonly ConfigurationStoreOptions _storeOptions;
        private readonly OperationalStoreOptions _operationalStore;

        [Fact]
        public async Task AddIdentityResourceAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IIdentityResourceRepository identityResourceRepository = new IdentityResourceRepository(context);

                var localizerIdentityResourceMock = new Mock<IStringLocalizer<IdentityResourceService>>();
                var localizerIdentityResource = localizerIdentityResourceMock.Object;

                IIdentityResourceService identityResourceService = new IdentityResourceService(identityResourceRepository, localizerIdentityResource);

                //Generate random new identity resource
                var identityResourceDto = IdentityResourceDtoMock.GenerateRandomIdentityResource(0);

                await identityResourceService.AddIdentityResourceAsync(identityResourceDto);

                //Get new identity resource
                var identityResource = await context.IdentityResources.Where(x => x.Name == identityResourceDto.Name).SingleOrDefaultAsync();

                var newIdentityResourceDto = await identityResourceService.GetIdentityResourceAsync(identityResource.Id);

                //Assert new identity resource
                identityResourceDto.ShouldBeEquivalentTo(newIdentityResourceDto, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task GetIdentityResourceAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IIdentityResourceRepository identityResourceRepository = new IdentityResourceRepository(context);

                var localizerIdentityResourceMock = new Mock<IStringLocalizer<IdentityResourceService>>();
                var localizerIdentityResource = localizerIdentityResourceMock.Object;

                IIdentityResourceService identityResourceService = new IdentityResourceService(identityResourceRepository, localizerIdentityResource);

                //Generate random new identity resource
                var identityResourceDto = IdentityResourceDtoMock.GenerateRandomIdentityResource(0);

                await identityResourceService.AddIdentityResourceAsync(identityResourceDto);

                //Get new identity resource
                var identityResource = await context.IdentityResources.Where(x => x.Name == identityResourceDto.Name).SingleOrDefaultAsync();

                var newIdentityResourceDto = await identityResourceService.GetIdentityResourceAsync(identityResource.Id);

                //Assert new identity resource
                identityResourceDto.ShouldBeEquivalentTo(newIdentityResourceDto, options => options.Excluding(o => o.Id));
            }
        }

        [Fact]
        public async Task RemoveIdentityResourceAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IIdentityResourceRepository identityResourceRepository = new IdentityResourceRepository(context);

                var localizerIdentityResourceMock = new Mock<IStringLocalizer<IdentityResourceService>>();
                var localizerIdentityResource = localizerIdentityResourceMock.Object;

                IIdentityResourceService identityResourceService = new IdentityResourceService(identityResourceRepository, localizerIdentityResource);

                //Generate random new identity resource
                var identityResourceDto = IdentityResourceDtoMock.GenerateRandomIdentityResource(0);

                await identityResourceService.AddIdentityResourceAsync(identityResourceDto);

                //Get new identity resource
                var identityResource = await context.IdentityResources.Where(x => x.Name == identityResourceDto.Name).SingleOrDefaultAsync();

                var newIdentityResourceDto = await identityResourceService.GetIdentityResourceAsync(identityResource.Id);

                //Assert new identity resource
                identityResourceDto.ShouldBeEquivalentTo(newIdentityResourceDto, options => options.Excluding(o => o.Id));

                //Remove identity resource
                await identityResourceService.DeleteIdentityResourceAsync(newIdentityResourceDto);

                //Try Get Removed identity resource
                var removeIdentityResource = await context.IdentityResources.Where(x => x.Id == identityResource.Id)
                    .SingleOrDefaultAsync();

                //Assert removed identity resource
                removeIdentityResource.Should().BeNull();
            }
        }

        [Fact]
        public async Task UpdateIdentityResourceAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                IIdentityResourceRepository identityResourceRepository = new IdentityResourceRepository(context);

                var localizerIdentityResourceMock = new Mock<IStringLocalizer<IdentityResourceService>>();
                var localizerIdentityResource = localizerIdentityResourceMock.Object;

                IIdentityResourceService identityResourceService = new IdentityResourceService(identityResourceRepository, localizerIdentityResource);

                //Generate random new identity resource
                var identityResourceDto = IdentityResourceDtoMock.GenerateRandomIdentityResource(0);

                await identityResourceService.AddIdentityResourceAsync(identityResourceDto);

                //Get new identity resource
                var identityResource = await context.IdentityResources.Where(x => x.Name == identityResourceDto.Name).SingleOrDefaultAsync();

                var newIdentityResourceDto = await identityResourceService.GetIdentityResourceAsync(identityResource.Id);

                //Assert new identity resource
                identityResourceDto.ShouldBeEquivalentTo(newIdentityResourceDto, options => options.Excluding(o => o.Id));

                //Detached the added item
                context.Entry(identityResource).State = EntityState.Detached;

                //Generete new identity resuorce with added item id
                var updatedIdentityResource = IdentityResourceDtoMock.GenerateRandomIdentityResource(identityResource.Id);

                //Update identity resuorce
                await identityResourceService.UpdateIdentityResourceAsync(updatedIdentityResource);

                var updatedIdentityResourceDto = await identityResourceService.GetIdentityResourceAsync(identityResource.Id);

                //Assert updated identity resuorce
                updatedIdentityResource.ShouldBeEquivalentTo(updatedIdentityResourceDto, options => options.Excluding(o => o.Id));
            }
        }
    }
}