using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Moq;
using Skoruba.IdentityServer4.Admin.Helpers.Identity;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Helpers
{
    public class IdentityErrorDescriberTests
    {
        [Theory]
        [ClassData(typeof(IdentityErrorDescriberTestData))]
        public void TranslationTests(string key, string methodName, string translated, params object[] args)
        {

            // Arrange
            var localizer = new Mock<IStringLocalizer<IdentityErrorMessages>>();

            // GetString extension method uses indexer underneath
            localizer.Setup(x => x[key])
                .Returns(new LocalizedString(key, translated));

            var describer = new IdentityErrorMessages(localizer.Object);

            // Act
            var methodInfo = typeof(IdentityErrorMessages).GetMethod(methodName); // get method name dynamically
            var error = methodInfo.Invoke(describer, args) as IdentityError; // invoke method on our instance

            // Assert
            Assert.IsType<IdentityError>(error);

            Assert.Equal(translated, error.Description);
            Assert.Equal(key, error.Code);
        }
    }
}
