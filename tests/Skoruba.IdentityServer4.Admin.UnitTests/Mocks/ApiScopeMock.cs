using System;
using Bogus;
using IdentityServer4.EntityFramework.Entities;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mocks
{
    public static class ApiScopeMock
    {
        public static Faker<ApiScopeClaim> GetApiScopeClaim(int id)
        {
            var fakerApiScopeClaim = new Faker<ApiScopeClaim>()
                .RuleFor(o => o.Type, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.Id, id);

            return fakerApiScopeClaim;
        }

        public static Faker<ApiScope> GetApiScopeFaker(int id)
        {
            var fakerApiScope = new Faker<ApiScope>()
                .RuleFor(o => o.Name, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.Id, id)
                .RuleFor(o => o.Description, f => f.Random.Words(f.Random.Number(1, 5)))
                .RuleFor(o => o.DisplayName, f => f.Random.Words(f.Random.Number(1, 5)))
                .RuleFor(o => o.UserClaims, f => GetApiScopeClaim(0).Generate(f.Random.Number(10)))
                .RuleFor(o => o.Emphasize, f => f.Random.Bool())
                .RuleFor(o => o.Required, f => f.Random.Bool())
                .RuleFor(o => o.ShowInDiscoveryDocument, f => f.Random.Bool());

            return fakerApiScope;
        }

        public static ApiScope GenerateRandomApiScope(int id)
        {
            var apiScope = GetApiScopeFaker(id).Generate();

            return apiScope;
        }
    }
}