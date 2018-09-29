using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Repositories;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Services
{
    public class IdentityServiceTests
    {
        public IdentityServiceTests()
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

        private IIdentityRepository<AdminDbContext, int, int, UserIdentity, UserIdentityRole, int,
            UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim,
            UserIdentityUserToken> GetIdentityRepository(AdminDbContext dbContext,
            UserManager<UserIdentity> userManager,
            RoleManager<UserIdentityRole> roleManager,
            IMapper mapper)
        {
            return new IdentityRepository<AdminDbContext, int, int, UserIdentity, UserIdentityRole, int,
                UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim,
                UserIdentityUserToken>(dbContext, userManager, roleManager, mapper);
        }

        private IIdentityService<AdminDbContext, UserDto<int>, int, RoleDto<int>, int, int, int, UserIdentity,
            UserIdentityRole, int,
            UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim,
            UserIdentityUserToken> GetIdentityService(IIdentityRepository<AdminDbContext, int, int, UserIdentity, UserIdentityRole, int, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken> identityRepository,
            IIdentityServiceResources identityServiceResources,
            IMapper mapper)
        {
            return new IdentityService<AdminDbContext, UserDto<int>, int, RoleDto<int>, int, int, int, UserIdentity,
                UserIdentityRole, int,
                UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim,
                UserIdentityUserToken>(identityRepository, identityServiceResources, mapper);
        }

        private IMapper GetMapper()
        {
            return new MapperConfiguration(cfg => cfg.AddProfile<IdentityMapperProfile<UserDto<int>, int, RoleDto<int>, int, UserIdentity, UserIdentityRole, int,
                        UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim,
                        UserIdentityUserToken>
                >())
                .CreateMapper();
        }

        private UserManager<UserIdentity> GetTestUserManager(AdminDbContext context)
        {
            var testUserManager = IdentityMock.TestUserManager(new UserStore<UserIdentity, UserIdentityRole, AdminDbContext, int, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityUserToken, UserIdentityRoleClaim>(context, new IdentityErrorDescriber()));

            return testUserManager;
        }

        private RoleManager<UserIdentityRole> GetTestRoleManager(AdminDbContext context)
        {
            var testRoleManager = IdentityMock.TestRoleManager(new RoleStore<UserIdentityRole, AdminDbContext, int, UserIdentityUserRole, UserIdentityRoleClaim>(context, new IdentityErrorDescriber()));

            return testRoleManager;
        }

        [Fact]
        public async Task AddUserAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var testUserManager = GetTestUserManager(context);
                var testRoleManager = GetTestRoleManager(context);
                var mapper = GetMapper();

                var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager, mapper);
                var localizerIdentityResource = new IdentityServiceResources();
                var identityService = GetIdentityService(identityRepository, localizerIdentityResource, mapper);

                //Generate random new user
                var userDto = IdentityDtoMock<int>.GenerateRandomUser(0);

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                userDto.ShouldBeEquivalentTo(newUserDto);
            }
        }

        [Fact]
        public async Task DeleteUserProviderAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var testUserManager = GetTestUserManager(context);
                var testRoleManager = GetTestRoleManager(context);

                var mapper = GetMapper();

                var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager, mapper);
                var localizerIdentityResource = new IdentityServiceResources();
                var identityService = GetIdentityService(identityRepository, localizerIdentityResource, mapper);

                //Generate random new user
                var userDto = IdentityDtoMock<int>.GenerateRandomUser(0);

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                userDto.ShouldBeEquivalentTo(newUserDto);

                var userProvider = IdentityMock.GenerateRandomUserProviders(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                    newUserDto.Id);

                //Add new user login
                await context.UserLogins.AddAsync(userProvider);
                await context.SaveChangesAsync();

                //Get added user provider
                var addedUserProvider = await context.UserLogins.Where(x => x.ProviderKey == userProvider.ProviderKey && x.LoginProvider == userProvider.LoginProvider).SingleOrDefaultAsync();
                addedUserProvider.Should().NotBeNull();

                var userProviderDto = IdentityDtoMock<int>.GenerateRandomUserProviders(userProvider.ProviderKey, userProvider.LoginProvider,
                    userProvider.UserId);

                await identityService.DeleteUserProvidersAsync(userProviderDto);

                //Get deleted user provider
                var deletedUserProvider = await context.UserLogins.Where(x => x.ProviderKey == userProvider.ProviderKey && x.LoginProvider == userProvider.LoginProvider).SingleOrDefaultAsync();
                deletedUserProvider.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddUserRoleAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var testUserManager = GetTestUserManager(context);
                var testRoleManager = GetTestRoleManager(context);

                var mapper = GetMapper();

                var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager, mapper);
                var localizerIdentityResource = new IdentityServiceResources();
                var identityService = GetIdentityService(identityRepository, localizerIdentityResource, mapper);

                //Generate random new user
                var userDto = IdentityDtoMock<int>.GenerateRandomUser(0);

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                userDto.ShouldBeEquivalentTo(newUserDto);

                //Generate random new role
                var roleDto = IdentityDtoMock<int>.GenerateRandomRole(0);

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                roleDto.ShouldBeEquivalentTo(newRoleDto);

                var userRoleDto = IdentityDtoMock<int>.GenerateRandomUserRole<RoleDto<int>>(roleDto.Id, userDto.Id);

                await identityService.CreateUserRoleAsync(userRoleDto);

                //Get new role
                var userRole = await context.UserRoles.Where(x => x.RoleId == roleDto.Id && x.UserId == userDto.Id).SingleOrDefaultAsync();

                userRole.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task DeleteUserRoleAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var testUserManager = GetTestUserManager(context);
                var testRoleManager = GetTestRoleManager(context);

                var mapper = GetMapper();

                var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager, mapper);
                var localizerIdentityResource = new IdentityServiceResources();
                var identityService = GetIdentityService(identityRepository, localizerIdentityResource, mapper);

                //Generate random new user
                var userDto = IdentityDtoMock<int>.GenerateRandomUser(0);

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                userDto.ShouldBeEquivalentTo(newUserDto);

                //Generate random new role
                var roleDto = IdentityDtoMock<int>.GenerateRandomRole(0);

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                roleDto.ShouldBeEquivalentTo(newRoleDto);

                var userRoleDto = IdentityDtoMock<int>.GenerateRandomUserRole<RoleDto<int>>(roleDto.Id, userDto.Id);

                await identityService.CreateUserRoleAsync(userRoleDto);

                //Get new role
                var userRole = await context.UserRoles.Where(x => x.RoleId == roleDto.Id && x.UserId == userDto.Id).SingleOrDefaultAsync();
                userRole.Should().NotBeNull();

                await identityService.DeleteUserRoleAsync(userRoleDto);

                //Get deleted role
                var userRoleDeleted = await context.UserRoles.Where(x => x.RoleId == roleDto.Id && x.UserId == userDto.Id).SingleOrDefaultAsync();
                userRoleDeleted.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddUserClaimAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var testUserManager = GetTestUserManager(context);
                var testRoleManager = GetTestRoleManager(context);

                var mapper = GetMapper();

                var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager, mapper);
                var localizerIdentityResource = new IdentityServiceResources();
                var identityService = GetIdentityService(identityRepository, localizerIdentityResource, mapper);

                //Generate random new user
                var userDto = IdentityDtoMock<int>.GenerateRandomUser(0);

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                userDto.ShouldBeEquivalentTo(newUserDto);

                //Generate random new user claim
                var userClaimDto = IdentityDtoMock<int>.GenerateRandomUserClaim(0, userDto.Id);

                await identityService.CreateUserClaimsAsync(userClaimDto);

                //Get new user claim
                var claim = await context.UserClaims.Where(x => x.ClaimType == userClaimDto.ClaimType && x.ClaimValue == userClaimDto.ClaimValue).SingleOrDefaultAsync();
                userClaimDto.ClaimId = claim.Id;

                var newUserClaim = await identityService.GetUserClaimAsync(userDto.Id.ToString(), claim.Id);

                //Assert new user claim
                userClaimDto.ShouldBeEquivalentTo(newUserClaim);
            }
        }

        [Fact]
        public async Task DeleteUserClaimAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var testUserManager = GetTestUserManager(context);
                var testRoleManager = GetTestRoleManager(context);

                var mapper = GetMapper();

                var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager, mapper);
                var localizerIdentityResource = new IdentityServiceResources();
                var identityService = GetIdentityService(identityRepository, localizerIdentityResource, mapper);

                //Generate random new user
                var userDto = IdentityDtoMock<int>.GenerateRandomUser(0);

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                userDto.ShouldBeEquivalentTo(newUserDto);

                //Generate random new user claim
                var userClaimDto = IdentityDtoMock<int>.GenerateRandomUserClaim(0, userDto.Id);

                await identityService.CreateUserClaimsAsync(userClaimDto);

                //Get new user claim
                var claim = await context.UserClaims.Where(x => x.ClaimType == userClaimDto.ClaimType && x.ClaimValue == userClaimDto.ClaimValue).SingleOrDefaultAsync();
                userClaimDto.ClaimId = claim.Id;

                var newUserClaim = await identityService.GetUserClaimAsync(userDto.Id.ToString(), claim.Id);

                //Assert new user claim
                userClaimDto.ShouldBeEquivalentTo(newUserClaim);

                await identityService.DeleteUserClaimsAsync(userClaimDto);

                //Get deleted user claim
                var deletedClaim = await context.UserClaims.Where(x => x.ClaimType == userClaimDto.ClaimType && x.ClaimValue == userClaimDto.ClaimValue).SingleOrDefaultAsync();
                deletedClaim.Should().BeNull();
            }
        }

        [Fact]
        public async Task UpdateUserAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var testUserManager = GetTestUserManager(context);
                var testRoleManager = GetTestRoleManager(context);

                var mapper = GetMapper();

                var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager, mapper);
                var localizerIdentityResource = new IdentityServiceResources();
                var identityService = GetIdentityService(identityRepository, localizerIdentityResource, mapper);

                //Generate random new user
                var userDto = IdentityDtoMock<int>.GenerateRandomUser(0);

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                userDto.ShouldBeEquivalentTo(newUserDto);

                //Detached the added item
                context.Entry(user).State = EntityState.Detached;

                //Generete new user with added item id
                var userDtoForUpdate = IdentityDtoMock<int>.GenerateRandomUser(user.Id);

                //Update user
                await identityService.UpdateUserAsync(userDtoForUpdate);

                var updatedUser = await identityService.GetUserAsync(userDtoForUpdate.Id.ToString());

                //Assert updated user
                userDtoForUpdate.ShouldBeEquivalentTo(updatedUser);
            }
        }

        [Fact]
        public async Task DeleteUserAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var testUserManager = GetTestUserManager(context);
                var testRoleManager = GetTestRoleManager(context);

                var mapper = GetMapper();

                var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager, mapper);
                var localizerIdentityResource = new IdentityServiceResources();
                var identityService = GetIdentityService(identityRepository, localizerIdentityResource, mapper);

                //Generate random new user
                var userDto = IdentityDtoMock<int>.GenerateRandomUser(0);

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                userDto.ShouldBeEquivalentTo(newUserDto);

                //Remove user
                await identityService.DeleteUserAsync(newUserDto.Id.ToString(), newUserDto);

                //Try Get Removed user
                var removeUser = await context.Users.Where(x => x.Id == user.Id)
                    .SingleOrDefaultAsync();

                //Assert removed user
                removeUser.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddRoleAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var testUserManager = GetTestUserManager(context);
                var testRoleManager = GetTestRoleManager(context);

                var mapper = GetMapper();

                var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager, mapper);
                var localizerIdentityResource = new IdentityServiceResources();
                var identityService = GetIdentityService(identityRepository, localizerIdentityResource, mapper);

                //Generate random new role
                var roleDto = IdentityDtoMock<int>.GenerateRandomRole(0);

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                roleDto.ShouldBeEquivalentTo(newRoleDto);
            }
        }

        [Fact]
        public async Task UpdateRoleAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var testUserManager = GetTestUserManager(context);
                var testRoleManager = GetTestRoleManager(context);

                var mapper = GetMapper();

                var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager, mapper);
                var localizerIdentityResource = new IdentityServiceResources();
                var identityService = GetIdentityService(identityRepository, localizerIdentityResource, mapper);

                //Generate random new role
                var roleDto = IdentityDtoMock<int>.GenerateRandomRole(0);

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                roleDto.ShouldBeEquivalentTo(newRoleDto);

                //Detached the added item
                context.Entry(role).State = EntityState.Detached;

                //Generete new role with added item id
                var roleDtoForUpdate = IdentityDtoMock<int>.GenerateRandomRole(role.Id);

                //Update role
                await identityService.UpdateRoleAsync(roleDtoForUpdate);

                var updatedRole = await identityService.GetRoleAsync(roleDtoForUpdate.Id.ToString());

                //Assert updated role
                roleDtoForUpdate.ShouldBeEquivalentTo(updatedRole);
            }
        }

        [Fact]
        public async Task DeleteRoleAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var testUserManager = GetTestUserManager(context);
                var testRoleManager = GetTestRoleManager(context);

                var mapper = GetMapper();

                var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager, mapper);
                var localizerIdentityResource = new IdentityServiceResources();
                var identityService = GetIdentityService(identityRepository, localizerIdentityResource, mapper);

                //Generate random new role
                var roleDto = IdentityDtoMock<int>.GenerateRandomRole(0);

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                roleDto.ShouldBeEquivalentTo(newRoleDto);

                //Remove role
                await identityService.DeleteRoleAsync(newRoleDto);

                //Try Get Removed role
                var removeRole = await context.Roles.Where(x => x.Id == role.Id)
                    .SingleOrDefaultAsync();

                //Assert removed role
                removeRole.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddRoleClaimAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var testUserManager = GetTestUserManager(context);
                var testRoleManager = GetTestRoleManager(context);

                var mapper = GetMapper();

                var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager, mapper);
                var localizerIdentityResource = new IdentityServiceResources();
                var identityService = GetIdentityService(identityRepository, localizerIdentityResource, mapper);

                //Generate random new role
                var roleDto = IdentityDtoMock<int>.GenerateRandomRole(0);

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                roleDto.ShouldBeEquivalentTo(newRoleDto);

                //Generate random new role claim
                var roleClaimDto = IdentityDtoMock<int>.GenerateRandomRoleClaim(0, roleDto.Id);

                await identityService.CreateRoleClaimsAsync(roleClaimDto);

                //Get new role claim
                var roleClaim = await context.RoleClaims.Where(x => x.ClaimType == roleClaimDto.ClaimType && x.ClaimValue == roleClaimDto.ClaimValue).SingleOrDefaultAsync();
                roleClaimDto.ClaimId = roleClaim.Id;

                var newRoleClaimDto = await identityService.GetRoleClaimAsync(roleDto.Id.ToString(), roleClaimDto.ClaimId);

                //Assert new role
                roleClaimDto.ShouldBeEquivalentTo(newRoleClaimDto, options => options.Excluding(o => o.RoleName));
            }
        }

        [Fact]
        public async Task RemoveRoleClaimAsync()
        {
            using (var context = new AdminDbContext(_dbContextOptions, _storeOptions, _operationalStore))
            {
                var testUserManager = GetTestUserManager(context);
                var testRoleManager = GetTestRoleManager(context);

                var mapper = GetMapper();

                var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager, mapper);
                var localizerIdentityResource = new IdentityServiceResources();
                var identityService = GetIdentityService(identityRepository, localizerIdentityResource, mapper);

                //Generate random new role
                var roleDto = IdentityDtoMock<int>.GenerateRandomRole(0);

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                roleDto.ShouldBeEquivalentTo(newRoleDto);

                //Generate random new role claim
                var roleClaimDto = IdentityDtoMock<int>.GenerateRandomRoleClaim(0, roleDto.Id);

                await identityService.CreateRoleClaimsAsync(roleClaimDto);

                //Get new role claim
                var roleClaim = await context.RoleClaims.Where(x => x.ClaimType == roleClaimDto.ClaimType && x.ClaimValue == roleClaimDto.ClaimValue).SingleOrDefaultAsync();
                roleClaimDto.ClaimId = roleClaim.Id;

                var newRoleClaimDto = await identityService.GetRoleClaimAsync(roleDto.Id.ToString(), roleClaimDto.ClaimId);

                //Assert new role
                roleClaimDto.ShouldBeEquivalentTo(newRoleClaimDto, options => options.Excluding(o => o.RoleName));

                await identityService.DeleteRoleClaimsAsync(roleClaimDto);

                var roleClaimToDelete = await context.RoleClaims.Where(x => x.ClaimType == roleClaimDto.ClaimType && x.ClaimValue == roleClaimDto.ClaimValue).SingleOrDefaultAsync();

                //Assert removed role claim
                roleClaimToDelete.Should().BeNull();
            }
        }
    }
}

