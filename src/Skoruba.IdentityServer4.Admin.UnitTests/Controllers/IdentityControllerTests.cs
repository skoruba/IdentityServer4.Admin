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
using Skoruba.IdentityServer4.Admin.Controllers;
using Skoruba.IdentityServer4.Admin.Data.DbContexts;
using Skoruba.IdentityServer4.Admin.Data.Entities.Identity;
using Skoruba.IdentityServer4.Admin.Data.Repositories;
using Skoruba.IdentityServer4.Admin.Services;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
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

            services.AddIdentity<UserIdentity, UserIdentityRole>()
                .AddEntityFrameworkStores<AdminDbContext>()
                .AddDefaultTokenProviders();

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
