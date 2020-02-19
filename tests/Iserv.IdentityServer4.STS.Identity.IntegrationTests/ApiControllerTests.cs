using FluentAssertions;
using Iserv.IdentityServer4.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Test;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Tests.Base;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Iserv.IdentityServer4.BusinessLogic.Settings;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Iserv.IdentityServer4.STS.Identity.IntegrationTests
{
    public class ApiControllerTests : BaseClassFixture
    {
        private readonly string _domain = "http://127.0.0.1.xip.io"; // "http://dev.it-serv.ru:5000";
        private readonly string _phone = "+79373732201";

        public ApiControllerTests(WebApplicationFactory<StartupTest> factory) : base(factory)
        {
        }
        
        // private IPortalService GetPortalService()
        // {
        //     var loggerMock = new Mock<ILogger<PortalService>>();
        //     return new PortalService(new AuthPortalOptions(), _serviceProvider.GetService<IHttpClientFactory>(),
        //         _memoryCache, loggerMock.Object);
        // }

        [Fact]
        public async Task UserInfo()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_domain);
                httpClient.DefaultRequestHeaders.Clear();
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", "iserv_mlk_mobile"),
                    new KeyValuePair<string, string>("client_secret", "mlk"),
                    new KeyValuePair<string, string>("redirect_uri", "http://dev.it-serv.ru"),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", "alexandr.chbk.test@yandex.ru"),
                    new KeyValuePair<string, string>("password", "12345678")
                });
                var response = await httpClient.PostAsync($"/connect/token", content);
                response.EnsureSuccessStatusCode();
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task RequestCheckPhoneAsync()
        {
            Client.BaseAddress = new Uri(_domain);
            Client.DefaultRequestHeaders.Clear();
            var _accountService = new Mock<IAccountService<UserIdentity, string>>();
            _accountService.Setup(s => s.FindByPhoneAsync(_phone)).ReturnsAsync(new UserIdentity()
            {
                Id = Guid.NewGuid().ToString(),
                Idext = Guid.NewGuid(),
                PhoneNumber = _phone
            });
            
        }
    }
}