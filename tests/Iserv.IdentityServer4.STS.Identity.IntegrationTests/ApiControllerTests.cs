using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Test;
using Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Tests.Base;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Iserv.IdentityServer4.STS.Identity.IntegrationTests
{
    public class ApiControllerTests : BaseClassFixture
    {
        public readonly string _domain = "http://127.0.0.1.xip.io"; // "http://dev.it-serv.ru:5000";

        public ApiControllerTests(WebApplicationFactory<StartupTest> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Userinfo()
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
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("client_id", "iserv_mlk_mobile"),
                    new KeyValuePair<string, string>("client_secret", "mlk"),
                    new KeyValuePair<string, string>("redirect_uri", "http://dev.it-serv.ru"),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", "alexandr.chbk.test@yandex.ru"),
                    new KeyValuePair<string, string>("password", "12345678")
                });
            var response = await Client.PostAsync($"/api/requestCheckPhone", new StringContent("+79373732201"));
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
        }
    }
}
