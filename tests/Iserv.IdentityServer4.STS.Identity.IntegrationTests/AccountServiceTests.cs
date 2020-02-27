using Iserv.IdentityServer4.BusinessLogic.Services;
using Moq;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Iserv.IdentityServer4.BusinessLogic.ExceptionHandling;
using Iserv.IdentityServer4.BusinessLogic.Models;
using Iserv.IdentityServer4.BusinessLogic.Sms;
using Iserv.IdentityServer4.STS.Identity.IntegrationTests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Iserv.IdentityServer4.STS.Identity.IntegrationTests
{
    public class AccountServiceTests
    {
        private readonly Guid _idext = Guid.NewGuid();
        private readonly string _email = "test@yandex.ru";
        private readonly string _phone = "+79373733333";
        private readonly string _password = "1234Qwer!";

        private readonly AdminIdentityDbContext _dbContext;
        private readonly ServiceProvider _serviceProvider;
        private readonly IMemoryCache _memoryCache;

        public AccountServiceTests()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
            var efServiceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
            services.AddOptions();
            services.AddDbContext<AdminIdentityDbContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(efServiceProvider));
            services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor {HttpContext = new DefaultHttpContext()});
            services.AddIdentity<UserIdentity, UserIdentityRole>()
                .AddEntityFrameworkStores<AdminIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddMemoryCache();
            var emailSenderMock = new Mock<IEmailSender>();
            var smsSenderMock = new Mock<ISmsService>();
            services.AddSingleton<IConfirmService, ConfirmService>(provider =>
                new ConfirmService(new TimeSpan(1000000000), _memoryCache, emailSenderMock.Object, smsSenderMock.Object));

            _serviceProvider = services.BuildServiceProvider();
            _memoryCache = _serviceProvider.GetService<IMemoryCache>();
            _dbContext = _serviceProvider.GetService<AdminIdentityDbContext>();
        }

        private UserManager<UserIdentity> GetTestUserManager(AdminIdentityDbContext context)
        {
            var testUserManager = IdentityMock.TestUserManager(
                new UserStore<UserIdentity, UserIdentityRole, AdminIdentityDbContext, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin,
                    UserIdentityUserToken, UserIdentityRoleClaim>(context, new IdentityErrorDescriber()));

            testUserManager.RegisterTokenProvider("Default", new EmailTokenProvider<UserIdentity>());
            testUserManager.RegisterTokenProvider("Phone", new PhoneNumberTokenProvider<UserIdentity>());

            return testUserManager;
        }

        private IPortalService GetPortalServiceSuccess()
        {
            var portalServiceMock = new Mock<IPortalService>();
            var values = new Dictionary<string, object> {{"email", _email}, {"phone", _phone}};
            portalServiceMock.Setup(p => p.GetUserAsync(_idext)).ReturnsAsync(new PortalResult<Dictionary<string, object>>
            {
                Value = values
            });
            portalServiceMock.Setup(p => p.RegisterAsync(It.IsAny<PortalRegistrationData>())).ReturnsAsync(new PortalResult<Guid>() {Value = _idext});
            portalServiceMock.Setup(p => p.UpdateUserAsync(It.IsAny<Guid>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<IEnumerable<FileModel>>()))
                .Callback<Guid, Dictionary<string, object>, IEnumerable<FileModel>>((idext, p, f) =>
                {
                    foreach (var item in p)
                    {
                        if (values.ContainsKey(item.Key))
                        {
                            values[item.Key] = item.Value;
                        }
                        else
                        {
                            values.Add(item.Key, item.Value);
                        }
                    }
                }).ReturnsAsync(new PortalResult<Guid>() {Value = _idext});
            portalServiceMock.Setup(p => p.UpdatePasswordAsync(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(new PortalResult());
            portalServiceMock.Setup(p => p.RestorePasswordByEmailAsync(It.IsAny<string>())).ReturnsAsync(new PortalResult());
            return portalServiceMock.Object;
        }

        private IPortalService GetPortalServiceError()
        {
            var portalServiceMock = new Mock<IPortalService>();
            portalServiceMock.Setup(p => p.GetUserAsync(_idext)).ReturnsAsync(new PortalResult<Dictionary<string, object>> {IsError = true});
            portalServiceMock.Setup(p => p.RegisterAsync(It.IsAny<PortalRegistrationData>())).ReturnsAsync(new PortalResult<Guid>() {IsError = true});
            portalServiceMock.Setup(p => p.UpdateUserAsync(It.IsAny<Guid>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<IEnumerable<FileModel>>()))
                .ReturnsAsync(new PortalResult<Guid>() {IsError = true});
            portalServiceMock.Setup(p => p.UpdatePasswordAsync(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(new PortalResult<Guid>() {IsError = true});
            portalServiceMock.Setup(p => p.RestorePasswordByEmailAsync(It.IsAny<string>())).ReturnsAsync(new PortalResult<Guid>() {IsError = true});
            return portalServiceMock.Object;
        }

        private IAccountService<UserIdentity, string> GetAccountService(AdminIdentityDbContext context, UserManager<UserIdentity> userManager, bool success)
        {
            var emailSenderMock = new Mock<IEmailSender>();
            var loggerMock = new Mock<ILogger<AccountService<AdminIdentityDbContext, UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
                UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>>>();
            return new AccountService<AdminIdentityDbContext, UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin,
                UserIdentityRoleClaim, UserIdentityUserToken>(context, _serviceProvider.GetService<IHttpContextAccessor>(), userManager,
                success ? GetPortalServiceSuccess() : GetPortalServiceError(),
                emailSenderMock.Object, _serviceProvider.GetService<IConfirmService>(), MessageTemplatesMock.GetMessageTemplates(), loggerMock.Object
            );
        }

        private IAccountService<UserIdentity, string> GetAccountService(AdminIdentityDbContext context, bool success)
        {
            return GetAccountService(context, GetTestUserManager(context), success);
        }

        [Fact]
        public async Task CreateUserFromPortalAsync()
        {
            var accountService = GetAccountService(_dbContext, false);
            Assert.NotEqual(await accountService.CreateUserFromPortalAsync(_idext, _password), IdentityResult.Success);
            accountService = GetAccountService(_dbContext, true);
            Assert.Equal(await accountService.CreateUserFromPortalAsync(_idext, _password), IdentityResult.Success);
            Assert.NotEqual(await accountService.CreateUserFromPortalAsync(_idext, _password), IdentityResult.Success);
            var user = await accountService.FindByIdextAsync(Guid.NewGuid());
            Assert.Null(user);
            user = await accountService.FindByIdextAsync(_idext);
            Assert.NotNull(user);
            user = await accountService.FindByPhoneAsync("+79999999999");
            Assert.Null(user);
            user = await accountService.FindByPhoneAsync(_phone);
            Assert.NotNull(user);
        }

        [Fact]
        public async Task UpdateUserFromPortalAsync()
        {
            var accountService = GetAccountService(_dbContext, true);
            await accountService.RequestCheckPhoneAsync(_phone, false);
            var userModel = new RegisterUserModel()
            {
                Email = _email,
                PhoneNumber = _phone,
                Password = _password,
                SmsCode = _memoryCache.Get(_phone + "_phone")?.ToString()
            };
            await accountService.RegisterAsync(userModel);
            var user = await accountService.FindByPhoneAsync(_phone);
            Assert.NotNull(user);

            accountService = GetAccountService(_dbContext, false);
            Assert.NotEqual(await accountService.UpdateUserFromPortalAsync(_idext), IdentityResult.Success);
            accountService = GetAccountService(_dbContext, true);
            Assert.NotEqual(await accountService.UpdateUserFromPortalAsync(Guid.NewGuid()), IdentityResult.Success);
            Assert.Equal(await accountService.UpdateUserFromPortalAsync(_idext), IdentityResult.Success);
        }

        [Fact]
        public async Task CheckEmailAsync()
        {
            var accountService = GetAccountService(_dbContext, true);
            Assert.Equal(await accountService.CreateUserFromPortalAsync(_idext, _password), IdentityResult.Success);

            await Assert.ThrowsAsync<ValidationException>(() => accountService.RequestCheckEmailAsync("---"));
            await Assert.ThrowsAsync<ValidationException>(() => accountService.RequestCheckEmailAsync("test123@mail.ru"));
            await accountService.RequestCheckEmailAsync(_email);
            var emailCode = _memoryCache.Get(_email + "_email")?.ToString();
            Assert.Throws<ValidationException>(() => accountService.ValidEmailCode("---", emailCode));
            Assert.Throws<ValidationException>(() => accountService.ValidEmailCode("test123@mail.ru", emailCode));
            Assert.Throws<ValidationException>(() => accountService.ValidEmailCode(_email, "000000"));
            accountService.ValidEmailCode(_email, emailCode);
        }

        [Fact]
        public async Task CheckPhoneAsync()
        {
            var accountService = GetAccountService(_dbContext, true);
            Assert.Equal(await accountService.CreateUserFromPortalAsync(_idext, _password), IdentityResult.Success);

            await Assert.ThrowsAsync<ValidationException>(() => accountService.RequestCheckPhoneAsync("---"));
            await Assert.ThrowsAsync<ValidationException>(() => accountService.RequestCheckPhoneAsync("+79372222222"));
            await accountService.RequestCheckPhoneAsync(_phone);
            var smsCode = _memoryCache.Get(_phone + "_phone")?.ToString();
            Assert.Throws<ValidationException>(() => accountService.ValidSmsCode("---", smsCode));
            Assert.Throws<ValidationException>(() => accountService.ValidSmsCode("+79372222222", smsCode));
            Assert.Throws<ValidationException>(() => accountService.ValidSmsCode(_phone, "000000"));
            accountService.ValidSmsCode(_phone, smsCode);
        }

        [Fact]
        public async Task RegisterAsync()
        {
            var accountService = GetAccountService(_dbContext, false);

            var model = new RegisterUserModel();
            await Assert.ThrowsAsync<ValidationException>(() => accountService.RegisterAsync(model));
            model.Email = _email;
            model.PhoneNumber = null;
            model.Password = null;
            await Assert.ThrowsAsync<ValidationException>(() => accountService.RegisterAsync(model));
            model.PhoneNumber = _phone;
            model.Password = _password;
            await Assert.ThrowsAsync<ValidationException>(() => accountService.RegisterAsync(model));
            model.SmsCode = "000000";
            await Assert.ThrowsAsync<ValidationException>(() => accountService.RegisterAsync(model));
            await accountService.RequestCheckPhoneAsync(_phone, false);
            model.SmsCode = _memoryCache.Get(_phone + "_phone")?.ToString();
            await Assert.ThrowsAsync<ValidationException>(() => accountService.RegisterAsync(model));

            accountService = GetAccountService(_dbContext, true);
            await accountService.RegisterAsync(model);
            await Assert.ThrowsAsync<ValidationException>(() => accountService.RegisterAsync(model));
            model.Email = "test@mail.ru";
            await Assert.ThrowsAsync<ValidationException>(() => accountService.RegisterAsync(model));
            model.Email = _email;
            model.PhoneNumber = "+79372222222";
            await Assert.ThrowsAsync<ValidationException>(() => accountService.RegisterAsync(model));
        }

        [Fact]
        public async Task UpdateUserAsync()
        {
            var emailNew = "test@mail.ru";
            var phoneNew = "+793722222222";
            var accountService = GetAccountService(_dbContext, true);
            await accountService.RequestCheckPhoneAsync(_phone, false);
            var userModel = new RegisterUserModel()
            {
                Email = _email,
                PhoneNumber = _phone,
                Password = _password,
                SmsCode = _memoryCache.Get(_phone + "_phone")?.ToString()
            };
            await accountService.RegisterAsync(userModel);
            var user = await accountService.FindByPhoneAsync(_phone);
            Assert.NotNull(user);

            accountService = GetAccountService(_dbContext, false);
            var model = new UpdateUserModel()
            {
                Id = Guid.Parse(user.Id),
                Values = new Dictionary<string, object>() {{"testing", true}},
                Files = new List<FileModel>() {new FileModel() {Name = "testing", Tag = "jpeg", FileData = new byte[] {0, 1, 2}}}
            };
            await Assert.ThrowsAsync<PortalException>(() => accountService.UpdateUserAsync(model));

            accountService = GetAccountService(_dbContext, true);
            model.Id = Guid.Empty;
            await Assert.ThrowsAsync<ValidationException>(() => accountService.UpdateUserAsync(model));
            model.Id = Guid.Parse(user.Id);
            model.Email = "111";
            await Assert.ThrowsAsync<ValidationException>(() => accountService.UpdateUserAsync(model));
            model.Email = emailNew;
            model.PhoneNumber = "123";
            await Assert.ThrowsAsync<ValidationException>(() => accountService.UpdateUserAsync(model));
            model.PhoneNumber = phoneNew;
            await Assert.ThrowsAsync<ValidationException>(() => accountService.UpdateUserAsync(model));

            await accountService.RequestCheckPhoneAsync(phoneNew, false);
            model.SmsCode = _memoryCache.Get(phoneNew + "_phone")?.ToString();
            await accountService.UpdateUserAsync(model);
            user = await accountService.FindByEmailAsync(emailNew);
            Assert.NotNull(user);
            user = await accountService.FindByPhoneAsync(phoneNew);
            Assert.NotNull(user);
            var extraFields = await accountService.GetExtraFieldsAsync(user);
            Assert.NotNull(extraFields);
            Assert.True(extraFields.ContainsKey("testing"));
            Assert.Equal("true", extraFields["testing"].ToLower());
        }

        [Fact]
        public async Task ChangeEmailAsync()
        {
            await CheckEmailAsync();

            var accountService = GetAccountService(_dbContext, true);
            var user = await accountService.FindByPhoneAsync(_phone);
            Assert.NotNull(user);
            await accountService.RequestCheckEmailAsync(_email, false);
            var code = _memoryCache.Get(_email + "_email")?.ToString();
            await Assert.ThrowsAsync<ValidationException>(() => accountService.ChangeEmailAsync(user, _email, code));

            var emailNew = "test123@mail.ru";
            await accountService.RequestCheckEmailAsync(emailNew, false);
            code = _memoryCache.Get(emailNew + "_email")?.ToString();
            await Assert.ThrowsAsync<ValidationException>(() => accountService.ChangeEmailAsync(null, _email, code));
            await Assert.ThrowsAsync<ValidationException>(() => accountService.ChangeEmailAsync(user, "test12345@mail.ru", code));
            await Assert.ThrowsAsync<ValidationException>(() => accountService.ChangeEmailAsync(user, _email, "000000"));
            await accountService.ChangeEmailAsync(user, emailNew, code);
        }

        [Fact]
        public async Task ChangePhoneAsync()
        {
            await CheckPhoneAsync();

            var accountService = GetAccountService(_dbContext, true);
            var user = await accountService.FindByPhoneAsync(_phone);
            Assert.NotNull(user);
            await accountService.RequestCheckPhoneAsync(_phone, false);
            var code = _memoryCache.Get(_phone + "_phone")?.ToString();
            await Assert.ThrowsAsync<ValidationException>(() => accountService.ChangePhoneAsync(user, _phone, code));

            var phoneNew = "79373732222";
            await accountService.RequestCheckPhoneAsync(phoneNew, false);
            code = _memoryCache.Get(phoneNew + "_phone")?.ToString();
            await Assert.ThrowsAsync<ValidationException>(() => accountService.ChangePhoneAsync(null, _phone, code));
            await Assert.ThrowsAsync<ValidationException>(() => accountService.ChangePhoneAsync(user, "+79372222222", code));
            await Assert.ThrowsAsync<ValidationException>(() => accountService.ChangePhoneAsync(user, _phone, "000000"));
            await accountService.ChangePhoneAsync(user, phoneNew, code);
        }

        [Fact]
        public async Task UpdatePasswordAsync()
        {
            var accountService = GetAccountService(_dbContext, false);
            await Assert.ThrowsAsync<ValidationException>(() => accountService.UpdatePasswordAsync(_idext, _password));
            accountService = GetAccountService(_dbContext, true);
            Assert.Equal(await accountService.CreateUserFromPortalAsync(_idext, _password), IdentityResult.Success);
            await Assert.ThrowsAsync<ValidationException>(() => accountService.UpdatePasswordAsync(Guid.NewGuid(), _password));
            await Assert.ThrowsAsync<ValidationException>(() => accountService.UpdatePasswordAsync(_idext, ""));
            await accountService.UpdatePasswordAsync(_idext, _password);
        }

        [Fact]
        public async Task RestorePasswordByEmailAsync()
        {
            var accountService = GetAccountService(_dbContext, false);
            await Assert.ThrowsAsync<ValidationException>(() => accountService.RestorePasswordByEmailAsync(_email));
            accountService = GetAccountService(_dbContext, true);
            Assert.Equal(await accountService.CreateUserFromPortalAsync(_idext, _password), IdentityResult.Success);
            await Assert.ThrowsAsync<ValidationException>(() => accountService.RestorePasswordByEmailAsync("123"));
            await Assert.ThrowsAsync<ValidationException>(() => accountService.RestorePasswordByEmailAsync("test123@mail.ru"));
            await accountService.RestorePasswordByEmailAsync(_email);
        }

        [Fact]
        public async Task RestorePasswordByPhoneAsync()
        {
            var accountService = GetAccountService(_dbContext, true);
            Assert.Equal(await accountService.CreateUserFromPortalAsync(_idext, _password), IdentityResult.Success);
            await Assert.ThrowsAsync<ValidationException>(() => accountService.RepairPasswordBySmsAsync("123"));
            await Assert.ThrowsAsync<ValidationException>(() => accountService.RepairPasswordBySmsAsync("+79333333333"));
            await accountService.RepairPasswordBySmsAsync(_phone);
            var code = _memoryCache.Get(_phone + "_pwd")?.ToString();
            Assert.Throws<ValidationException>(() => accountService.ValidSmsCodeChangePassword("+79333333333", code));
            Assert.Throws<ValidationException>(() => accountService.ValidSmsCodeChangePassword(_phone, "000000"));
            accountService.ValidSmsCodeChangePassword(_phone, code);
            await Assert.ThrowsAsync<ValidationException>(() => accountService.ChangePasswordBySmsAsync("+79333333333", code, "12345678"));
            await Assert.ThrowsAsync<ValidationException>(() => accountService.ChangePasswordBySmsAsync(_phone, "000000", "12345678"));
            await Assert.ThrowsAsync<ValidationException>(() => accountService.ChangePasswordBySmsAsync(_phone, code, ""));
            await accountService.ChangePasswordBySmsAsync(_phone, code, "12345678");
        }
    }
}