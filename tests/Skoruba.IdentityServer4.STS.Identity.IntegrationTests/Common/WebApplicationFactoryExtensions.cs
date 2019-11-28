﻿using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Skoruba.IdentityServer4.STS.Identity.IntegrationTests.Common
{
    public static class WebApplicationFactoryExtensions 
    {
        public static HttpClient SetupClient(this WebApplicationFactory<Startup> fixture)
        {
            var options = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };

            return fixture.WithWebHostBuilder(
                builder => builder
                    .UseEnvironment(Microsoft.Extensions.Hosting.Environments.Staging)
                    .ConfigureTestServices(services => { })
            ).CreateClient(options);
        }
    }
}