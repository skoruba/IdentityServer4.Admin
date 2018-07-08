using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.Controllers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities.Identity;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Skoruba.IdentityServer4.Admin.Helpers;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Controllers
{
    public class IdentityControllerTests
    {
        [Fact]
        public async Task AddUser()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var userDto = IdentityDtoMock.GenerateRandomUser(0);
            var result = await controller.User(userDto);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("Users");

            var user = await dbContext.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
            userDto.Id = user.Id;

            var addedUser = await identityService.GetUserAsync(userDto);

            userDto.ShouldBeEquivalentTo(addedUser, opts => opts.Excluding(x => x.Id));
        }

        [Fact]
        public async Task DeleteUser()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var userDto = IdentityDtoMock.GenerateRandomUser(0);

            await identityService.CreateUserAsync(userDto);

            var userId = await dbContext.Users.Where(x => x.UserName == userDto.UserName).Select(x => x.Id).SingleOrDefaultAsync();
            userDto.Id = userId;

            var result = await controller.UserDelete(userDto);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("Users");

            var user = await dbContext.Users.Where(x => x.Id == userDto.Id).SingleOrDefaultAsync();
            user.Should().BeNull();
        }

        [Fact]
        public async Task UpdateUser()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var userDto = IdentityDtoMock.GenerateRandomUser(0);
            await identityService.CreateUserAsync(userDto);

            //Get inserted userid
            var userId = await dbContext.Users.Where(x => x.UserName == userDto.UserName).Select(x => x.Id).SingleOrDefaultAsync();

            var updatedUserDto = IdentityDtoMock.GenerateRandomUser(userId);

            var result = await controller.User(updatedUserDto);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("Users");

            var updatedUser = await identityService.GetUserAsync(updatedUserDto);

            updatedUserDto.ShouldBeEquivalentTo(updatedUser, opts => opts.Excluding(x => x.Id));
        }

        [Fact]
        public async Task GetUser()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var userDto = IdentityDtoMock.GenerateRandomUser(0);

            //Add user
            await identityService.CreateUserAsync(userDto);

            //Get inserted userid
            var userId = await dbContext.Users.Where(x => x.UserName == userDto.UserName).Select(x => x.Id).SingleOrDefaultAsync();

            //Try call controller action
            var result = await controller.User(userId);

            // Assert            
            var viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewName.Should().Be("User");
            viewResult.ViewData.Should().NotBeNull();

            var viewModel = Assert.IsType<UserDto>(viewResult.ViewData.Model);
            userDto.Id = userId;
            var addedUser = await identityService.GetUserAsync(userDto);

            viewModel.ShouldBeEquivalentTo(addedUser);
        }

        [Fact]
        public async Task AddRole()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var roleDto = IdentityDtoMock.GenerateRandomRole(0);
            var result = await controller.Role(roleDto);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("Roles");

            var role = await dbContext.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
            roleDto.Id = role.Id;

            var addedRole = await identityService.GetRoleAsync(roleDto);

            roleDto.ShouldBeEquivalentTo(addedRole, opts => opts.Excluding(x => x.Id));
        }

        [Fact]
        public async Task GetRole()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var roleDto = IdentityDtoMock.GenerateRandomRole(0);
            await identityService.CreateRoleAsync(roleDto);

            var roleId = await dbContext.Roles.Where(x => x.Name == roleDto.Name).Select(x => x.Id).SingleOrDefaultAsync();
            var result = await controller.Role(roleId);

            // Assert            
            var viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewName.Should().BeNull();
            viewResult.ViewData.Should().NotBeNull();

            var viewModel = Assert.IsType<RoleDto>(viewResult.ViewData.Model);
            roleDto.Id = roleId;
            var addedRole = await identityService.GetRoleAsync(roleDto);

            viewModel.ShouldBeEquivalentTo(addedRole);
        }

        [Fact]
        public async Task DeleteRole()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var roleDto = IdentityDtoMock.GenerateRandomRole(0);

            await identityService.CreateRoleAsync(roleDto);

            var roleId = await dbContext.Roles.Where(x => x.Name == roleDto.Name).Select(x => x.Id).SingleOrDefaultAsync();
            roleDto.Id = roleId;

            var result = await controller.RoleDelete(roleDto);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("Roles");

            var role = await dbContext.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
            role.Should().BeNull();
        }

        [Fact]
        public async Task UpdateRole()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var roleDto = IdentityDtoMock.GenerateRandomRole(0);

            await identityService.CreateRoleAsync(roleDto);

            var roleId = await dbContext.Roles.Where(x => x.Name == roleDto.Name).Select(x => x.Id).SingleOrDefaultAsync();
            var updatedRoleDto = IdentityDtoMock.GenerateRandomRole(roleId);

            var result = await controller.Role(updatedRoleDto);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("Roles");

            var updatedRole = await identityService.GetRoleAsync(updatedRoleDto);

            updatedRoleDto.ShouldBeEquivalentTo(updatedRole, opts => opts.Excluding(x => x.Id));
        }

        [Fact]
        public async Task AddUserClaim()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var userDto = IdentityDtoMock.GenerateRandomUser(0);
            await identityService.CreateUserAsync(userDto);

            var user = await dbContext.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
            userDto.Id = user.Id;

            var userClaimsDto = IdentityDtoMock.GenerateRandomUserClaim(0, user.Id);
            var result = await controller.UserClaims(userClaimsDto);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("UserClaims");

            var userClaim = await dbContext.UserClaims.Where(x => x.ClaimValue == userClaimsDto.ClaimValue).SingleOrDefaultAsync();

            var addedUserClaim = await identityService.GetUserClaimAsync(user.Id, userClaim.Id);

            userClaimsDto.ShouldBeEquivalentTo(addedUserClaim, opts => opts.Excluding(x => x.ClaimId));
        }

        [Fact]
        public async Task AddUserRole()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var userDto = IdentityDtoMock.GenerateRandomUser(0);
            await identityService.CreateUserAsync(userDto);

            var roleDto = IdentityDtoMock.GenerateRandomRole(0);
            await identityService.CreateRoleAsync(roleDto);

            var user = await dbContext.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
            userDto.Id = user.Id;

            var role = await dbContext.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
            roleDto.Id = role.Id;

            var userRoleDto = IdentityDtoMock.GenerateRandomUserRole(roleDto.Id, user.Id);
            var result = await controller.UserRoles(userRoleDto);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("UserRoles");

            var userRole = await dbContext.UserRoles.Where(x => x.RoleId == roleDto.Id && x.UserId == userDto.Id).SingleOrDefaultAsync();

            userRoleDto.ShouldBeEquivalentTo(userRole, opts => opts.Excluding(x => x.Roles)
                                                                   .Excluding(x => x.RolesList)
                                                                   .Excluding(x => x.PageSize)
                                                                   .Excluding(x => x.TotalCount));
        }

        [Fact]
        public async Task DeleteUserRole()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var userDto = IdentityDtoMock.GenerateRandomUser(0);
            await identityService.CreateUserAsync(userDto);

            var roleDto = IdentityDtoMock.GenerateRandomRole(0);
            await identityService.CreateRoleAsync(roleDto);

            var user = await dbContext.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
            userDto.Id = user.Id;

            var role = await dbContext.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
            roleDto.Id = role.Id;

            var userRoleDto = IdentityDtoMock.GenerateRandomUserRole(roleDto.Id, user.Id);

            await identityService.CreateUserRoleAsync(userRoleDto);

            var result = await controller.UserRolesDelete(userRoleDto);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("UserRoles");

            var userRole = await dbContext.UserRoles.Where(x => x.RoleId == roleDto.Id && x.UserId == userDto.Id).SingleOrDefaultAsync();

            userRole.Should().BeNull();
        }

        [Fact]
        public async Task DeleteUserClaim()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var userDto = IdentityDtoMock.GenerateRandomUser(0);
            await identityService.CreateUserAsync(userDto);

            var user = await dbContext.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
            userDto.Id = user.Id;

            var userClaimsDto = IdentityDtoMock.GenerateRandomUserClaim(0, user.Id);
            await identityService.CreateUserClaimsAsync(userClaimsDto);
            var newUserClaim = await dbContext.UserClaims.Where(x => x.ClaimValue == userClaimsDto.ClaimValue).SingleOrDefaultAsync();
            userClaimsDto.ClaimId = newUserClaim.Id;

            var result = await controller.UserClaimsDelete(userClaimsDto);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("UserClaims");

            var userClaim = await dbContext.UserClaims.Where(x => x.ClaimValue == userClaimsDto.ClaimValue).SingleOrDefaultAsync();

            userClaim.Should().BeNull();
        }

        [Fact]
        public async Task AddRoleClaim()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var roleDto = IdentityDtoMock.GenerateRandomRole(0);
            await identityService.CreateRoleAsync(roleDto);

            var role = await dbContext.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
            roleDto.Id = role.Id;

            var roleClaimsDto = IdentityDtoMock.GenerateRandomRoleClaim(0, role.Id);
            var result = await controller.RoleClaims(roleClaimsDto);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("RoleClaims");

            var roleClaim = await dbContext.RoleClaims.Where(x => x.ClaimValue == roleClaimsDto.ClaimValue).SingleOrDefaultAsync();

            var addedRoleClaim = await identityService.GetRoleClaimAsync(role.Id, roleClaim.Id);

            roleClaimsDto.ShouldBeEquivalentTo(addedRoleClaim, opts => opts.Excluding(x => x.ClaimId));
        }

        [Fact]
        public async Task DeleteRoleClaim()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var roleDto = IdentityDtoMock.GenerateRandomRole(0);
            await identityService.CreateRoleAsync(roleDto);

            var role = await dbContext.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
            roleDto.Id = role.Id;

            var roleClaimsDto = IdentityDtoMock.GenerateRandomRoleClaim(0, role.Id);
            await identityService.CreateRoleClaimsAsync(roleClaimsDto);
            var newRoleClaim = await dbContext.RoleClaims.Where(x => x.ClaimValue == roleClaimsDto.ClaimValue).SingleOrDefaultAsync();
            roleClaimsDto.ClaimId = newRoleClaim.Id;

            var result = await controller.RoleClaimsDelete(roleClaimsDto);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("RoleClaims");

            var roleClaim = await dbContext.RoleClaims.Where(x => x.ClaimValue == roleClaimsDto.ClaimValue).SingleOrDefaultAsync();

            roleClaim.Should().BeNull();
        }

        [Fact]
        public async Task UserChangePassword()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var userDto = IdentityDtoMock.GenerateRandomUser(0);

            await identityService.CreateUserAsync(userDto);

            var userId = await dbContext.Users.Where(x => x.UserName == userDto.UserName).Select(x => x.Id).SingleOrDefaultAsync();

            var changePassword = IdentityDtoMock.GenerateRandomUserChangePassword(userId, "IdentityServer4!");

            var result = await controller.UserChangePassword(changePassword);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("User");

            var user = await dbContext.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
            user.PasswordHash.Should().NotBeNull();
        }

        [Fact]
        public async Task UserProvidersDelete()
        {
            //Get Services
            var serviceProvider = GetServices();
            var dbContext = serviceProvider.GetRequiredService<AdminDbContext>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            // Get controller
            var controller = PrepareIdentityController(serviceProvider);
            var userDto = IdentityDtoMock.GenerateRandomUser(0);
            await identityService.CreateUserAsync(userDto);

            var userId = await dbContext.Users.Where(x => x.UserName == userDto.UserName).Select(x => x.Id).SingleOrDefaultAsync();
            var randomProviderKey = Guid.NewGuid().ToString();
            var randomProviderLogin = Guid.NewGuid().ToString();

            var provider = IdentityMock.GenerateRandomUserProviders(randomProviderKey, randomProviderLogin, userId);

            await dbContext.UserLogins.AddAsync(provider);
            await dbContext.SaveChangesAsync();

            var providersDto = IdentityDtoMock.GenerateRandomUserProviders(randomProviderKey, randomProviderLogin, userId);
            var result = await controller.UserProvidersDelete(providersDto);

            // Assert            
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("UserProviders");

            var userProvider = await dbContext.UserLogins.Where(x => x.ProviderKey == randomProviderKey).SingleOrDefaultAsync();
            userProvider.Should().BeNull();
        }

        private IdentityController PrepareIdentityController(IServiceProvider serviceProvider)
        {
            // Arrange
            var localizer = serviceProvider.GetRequiredService<IStringLocalizer<IdentityController>>();
            var logger = serviceProvider.GetRequiredService<ILogger<ConfigurationController>>();
            var tempDataDictionaryFactory = serviceProvider.GetRequiredService<ITempDataDictionaryFactory>();
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();

            //Get Controller
            var controller = new IdentityController(identityService, logger, localizer);

            //Setup TempData for notofication in basecontroller
            var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            var tempData = tempDataDictionaryFactory.GetTempData(httpContext);
            controller.TempData = tempData;

            return controller;
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

            //Add Admin services
            services.AddServices();

            services.AddIdentity<UserIdentity, UserIdentityRole>()
                .AddEntityFrameworkStores<AdminDbContext>()
                .AddDefaultTokenProviders();

            services.AddSession();
            services.AddMvc()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
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
