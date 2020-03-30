using System;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Moq;
using Skoruba.AuditLogging.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Services
{
    public class PersistedGrantServiceTests
    {
        public PersistedGrantServiceTests()
        {
            var databaseName = Guid.NewGuid().ToString();
            var identityDatabaseName = Guid.NewGuid().ToString();

            _dbContextOptions = new DbContextOptionsBuilder<IdentityServerPersistedGrantDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            _identityDbContextOptions = new DbContextOptionsBuilder<AdminIdentityDbContext>()
                .UseInMemoryDatabase(identityDatabaseName)
                .Options;

            _storeOptions = new ConfigurationStoreOptions();
            _operationalStore = new OperationalStoreOptions();
        }

        private readonly DbContextOptions<AdminIdentityDbContext> _identityDbContextOptions;
        private readonly DbContextOptions<IdentityServerPersistedGrantDbContext> _dbContextOptions;
        private readonly ConfigurationStoreOptions _storeOptions;
        private readonly OperationalStoreOptions _operationalStore;

        private IPersistedGrantAspNetIdentityRepository GetPersistedGrantRepository(AdminIdentityDbContext identityDbContext, IdentityServerPersistedGrantDbContext context)
        {
            var persistedGrantRepository = new PersistedGrantAspNetIdentityRepository<AdminIdentityDbContext, IdentityServerPersistedGrantDbContext, UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>(identityDbContext, context);

            return persistedGrantRepository;
        }

        private IPersistedGrantAspNetIdentityService
            GetPersistedGrantService(IPersistedGrantAspNetIdentityRepository repository, IPersistedGrantAspNetIdentityServiceResources persistedGrantServiceResources, IAuditEventLogger auditEventLogger)
        {
            var persistedGrantService = new PersistedGrantAspNetIdentityService(repository,
                persistedGrantServiceResources, auditEventLogger);

            return persistedGrantService;
        }

        [Fact]
        public async Task GetPersistedGrantAsync()
        {
            using (var context = new IdentityServerPersistedGrantDbContext(_dbContextOptions, _operationalStore))
            {
                using (var identityDbContext = new AdminIdentityDbContext(_identityDbContextOptions))
                {
                    var persistedGrantRepository = GetPersistedGrantRepository(identityDbContext, context);

                    var localizerMock = new Mock<IPersistedGrantAspNetIdentityServiceResources>();
                    var localizer = localizerMock.Object;

                    var auditLoggerMock = new Mock<IAuditEventLogger>();
                    var auditLogger = auditLoggerMock.Object;
                    
                    var persistedGrantService = GetPersistedGrantService(persistedGrantRepository, localizer, auditLogger);

                    //Generate persisted grant
                    var persistedGrantKey = Guid.NewGuid().ToString();
                    var persistedGrant = PersistedGrantMock.GenerateRandomPersistedGrant(persistedGrantKey);

                    //Try add new persisted grant
                    await context.PersistedGrants.AddAsync(persistedGrant);
                    await context.SaveChangesAsync();

                    //Try get persisted grant
                    var persistedGrantAdded = await persistedGrantService.GetPersistedGrantAsync(persistedGrantKey);

                    //Assert
                    persistedGrant.ShouldBeEquivalentTo(persistedGrantAdded);
                }
            }
        }

        [Fact]
        public async Task DeletePersistedGrantAsync()
        {
            using (var context = new IdentityServerPersistedGrantDbContext(_dbContextOptions, _operationalStore))
            {
                using (var identityDbContext = new AdminIdentityDbContext(_identityDbContextOptions))
                {
                    var persistedGrantRepository = GetPersistedGrantRepository(identityDbContext, context);

                    var localizerMock = new Mock<IPersistedGrantAspNetIdentityServiceResources>();
                    var localizer = localizerMock.Object;

                    var auditLoggerMock = new Mock<IAuditEventLogger>();
                    var auditLogger = auditLoggerMock.Object;

                    var persistedGrantService = GetPersistedGrantService(persistedGrantRepository, localizer, auditLogger);

                    //Generate persisted grant
                    var persistedGrantKey = Guid.NewGuid().ToString();
                    var persistedGrant = PersistedGrantMock.GenerateRandomPersistedGrant(persistedGrantKey);

                    //Try add new persisted grant
                    await context.PersistedGrants.AddAsync(persistedGrant);
                    await context.SaveChangesAsync();

                    //Try delete persisted grant
                    await persistedGrantService.DeletePersistedGrantAsync(persistedGrantKey);

                    var grant = await persistedGrantRepository.GetPersistedGrantAsync(persistedGrantKey);

                    //Assert
                    grant.Should().BeNull();
                }
            }
        }

        [Fact]
        public async Task DeletePersistedGrantsAsync()
        {
            using (var context = new IdentityServerPersistedGrantDbContext(_dbContextOptions, _operationalStore))
            {
                using (var identityDbContext = new AdminIdentityDbContext(_identityDbContextOptions))
                {
                    var persistedGrantRepository = GetPersistedGrantRepository(identityDbContext, context);

                    var localizerMock = new Mock<IPersistedGrantAspNetIdentityServiceResources>();
                    var localizer = localizerMock.Object;

                    var auditLoggerMock = new Mock<IAuditEventLogger>();
                    var auditLogger = auditLoggerMock.Object;

                    var persistedGrantService = GetPersistedGrantService(persistedGrantRepository, localizer, auditLogger);

                    const int subjectId = 1;

                    for (var i = 0; i < 4; i++)
                    {
                        //Generate persisted grant
                        var persistedGrantKey = Guid.NewGuid().ToString();
                        var persistedGrant = PersistedGrantMock.GenerateRandomPersistedGrant(persistedGrantKey, subjectId.ToString());

                        //Try add new persisted grant
                        await context.PersistedGrants.AddAsync(persistedGrant);
                    }

                    await context.SaveChangesAsync();

                    //Try delete persisted grant
                    await persistedGrantService.DeletePersistedGrantsAsync(subjectId.ToString());

                    var grant = await persistedGrantRepository.GetPersistedGrantsByUserAsync(subjectId.ToString());

                    //Assert
                    grant.TotalCount.Should().Be(0);
                }
            }
        }
    }
}