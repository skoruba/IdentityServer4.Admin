using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Test;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Common
{
    public static class WebApplicationFactoryExtensions 
    {
        public static HttpClient SetupClient(this WebApplicationFactory<StartupTestSingleTenant> fixture)
        {
            var options = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };

            return fixture.WithWebHostBuilder(
                builder => builder
                    .UseStartup<StartupTestSingleTenant>()
                    .ConfigureTestServices(services => { })
            ).CreateClient(options);
        }
        public static HttpClient SetupClient(this WebApplicationFactory<StartupTestMultiTenant> fixture)
        {
            var options = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = false
            };

            return fixture.WithWebHostBuilder(
                builder => builder
                    .UseStartup<StartupTestMultiTenant>()
                    .ConfigureTestServices(services => { })
            ).CreateClient(options);
        }
    }
}