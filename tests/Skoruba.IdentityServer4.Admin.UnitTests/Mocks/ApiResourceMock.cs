using System;
using System.Collections.Generic;
using Bogus;
using IdentityServer4.EntityFramework.Entities;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mocks
{
    public static class ApiResourceMock
    {
        public static Faker<ApiResource> GetApiResourceFaker(int id)
        {            
            var fakerApiResource = new Faker<ApiResource>()
                .RuleFor(o => o.Name, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.Id, id)
                .RuleFor(o => o.Description, f => f.Random.Words(f.Random.Number(1, 5)))
                .RuleFor(o => o.DisplayName, f => f.Random.Words(f.Random.Number(1, 5)))
                .RuleFor(o => o.Enabled, f => f.Random.Bool())
                .RuleFor(o => o.Scopes, new List<ApiScope>()) //Api Scopes are managed with seperate method
                .RuleFor(o => o.Secrets, new List<ApiSecret>()) //Api Secret are managed with seperate method
                .RuleFor(o => o.UserClaims, f => GetApiResourceClaimFaker(0).Generate(f.Random.Number(10)));

            return fakerApiResource;
        }

        public static Faker<ApiSecret> GetApiSecretFaker(int id)
        {
            var fakerApiSecret = new Faker<ApiSecret>()
                .RuleFor(o => o.Type, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.Value, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.Id, id)
                .RuleFor(o => o.Description, f => f.Random.Words(f.Random.Number(1, 5)))
                .RuleFor(o => o.Expiration, f => f.Date.Future());                

            return fakerApiSecret;
        }

        public static Faker<ApiResourceClaim> GetApiResourceClaimFaker(int id)
        {
            var fakerApiResourceClaim = new Faker<ApiResourceClaim>()
                .RuleFor(o => o.Type, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.Id, id);

            return fakerApiResourceClaim;
        }

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

        public static ApiResource GenerateRandomApiResource(int id)
        {
            var apiResource = GetApiResourceFaker(id).Generate();

            return apiResource;
        }

        public static ApiScope GenerateRandomApiScope(int id)
        {
            var apiScope = GetApiScopeFaker(id).Generate();

            return apiScope;
        }

        public static ApiSecret GenerateRandomApiSecret(int id)
        {
            var apiSecret = GetApiSecretFaker(id).Generate();

            return apiSecret;
        }
    }
}
