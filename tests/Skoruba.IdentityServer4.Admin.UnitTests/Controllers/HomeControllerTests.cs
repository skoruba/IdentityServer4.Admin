using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.Controllers;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Controllers
{
    public class HomeControllerTests
    {
        private readonly IServiceProvider _serviceProvider;

        public HomeControllerTests()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void GetIndex()
        {
            // Arrange
            var logger = _serviceProvider.GetRequiredService<ILogger<ConfigurationController>>();

            var controller = new HomeController(logger);

            // Action
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);

            Assert.NotNull(viewResult.ViewData);
        }
    }
}
