using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.Configuration.Test;

namespace Skoruba.IdentityServer4.Admin.IntegrationTests.Common
{
    public static class WebApplicationFactoryExtensions
    {
        public static HttpClient SetupClient(this WebApplicationFactory<StartupTestSingleTenant> fixture)
        {
            var options = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };

            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.singletenant.json");

            return fixture.WithWebHostBuilder(
                builder => builder
                    .UseStartup<StartupTestSingleTenant>()
                    .ConfigureTestServices(services => { })
                    .ConfigureAppConfiguration((context, conf) =>
                    {
                        conf.AddJsonFile(configPath, optional: false);
                    })
           ).CreateClient(options);

            //return fixture.WithWebHostBuilder(
            //    builder => builder
            //        .UseStartup<StartupTestSingleTenant>()
            //        .ConfigureTestServices(services => { })
            //).CreateClient(options);
        }
        public static HttpClient SetupClient(this WebApplicationFactory<StartupTestMultiTenant> fixture)
        {
            var options = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };

            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.multitenant.json");

            return fixture.WithWebHostBuilder(
                builder => builder
                    .UseStartup<StartupTestSingleTenant>()
                    .ConfigureTestServices(services => { })
                    .ConfigureAppConfiguration((context, conf) =>
                    {
                        conf.AddJsonFile(configPath, optional: false);
                    })
           ).CreateClient(options);

            //return fixture.WithWebHostBuilder(
            //    builder => builder
            //        .UseStartup<StartupTest>()
            //        .ConfigureTestServices(services => { })
            //).CreateClient(options);
        }
    }
}