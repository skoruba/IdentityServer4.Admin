using System;
using Bogus;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mocks
{
    public static class IdentityMock
    {
        public static Faker<UserIdentityUserLogin> GetUserProvidersFaker(string key, string loginProvider, int userId)
        {
            var userProvidersFaker = new Faker<UserIdentityUserLogin>()
                .RuleFor(o => o.LoginProvider, f => loginProvider)
                .RuleFor(o => o.ProviderKey, f => key)
                .RuleFor(o => o.ProviderDisplayName, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.UserId, userId);

            return userProvidersFaker;
        }

        public static UserIdentityUserLogin GenerateRandomUserProviders(string key, string loginProvider, int userId)
        {
            var provider = GetUserProvidersFaker(key, loginProvider, userId).Generate();

            return provider;
        }
    }
}
