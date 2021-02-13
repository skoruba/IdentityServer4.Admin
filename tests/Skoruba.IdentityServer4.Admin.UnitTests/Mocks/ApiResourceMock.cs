using System;
using System.Collections.Generic;
using System.Linq;
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
                .RuleFor(o => o.Scopes, new List<ApiResourceScope>()) //Api Scopes are managed with separate method
                .RuleFor(o => o.Secrets, new List<ApiResourceSecret>()) //Api Secret are managed with separate method
                .RuleFor(o => o.UserClaims, f => GetApiResourceClaimFaker(0).Generate(f.Random.Number(10)))
                .RuleFor(o => o.ShowInDiscoveryDocument, f => f.Random.Bool())
                .RuleFor(o => o.AllowedAccessTokenSigningAlgorithms, f => string.Join(",", ClientMock.AllowedSigningAlgorithms().Take(f.Random.Number(1, 5)).ToList()));

            return fakerApiResource;
        }

        public static Faker<ApiResourceSecret> GetApiSecretFaker(int id)
        {
            var fakerApiSecret = new Faker<ApiResourceSecret>()
                .RuleFor(o => o.Type, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.Value, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.Id, id)
                .RuleFor(o => o.Description, f => f.Random.Words(f.Random.Number(1, 5)))
                .RuleFor(o => o.Expiration, f => f.Date.Future());

            return fakerApiSecret;
        }

        public static ApiResourceProperty GenerateRandomApiResourceProperty(int id)
        {
            var apiResourcePropertyFaker = ApiResourcePropertyFaker(id);

            var apiResourceProperty = apiResourcePropertyFaker.Generate();

            return apiResourceProperty;
        }

        public static Faker<ApiResourceProperty> ApiResourcePropertyFaker(int id)
        {
            var resourcePropertyFaker = new Faker<ApiResourceProperty>()
                .StrictMode(false)
                .RuleFor(o => o.Id, id)
                .RuleFor(o => o.Key, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.Value, f => f.Random.Words(f.Random.Number(1, 5)));

            return resourcePropertyFaker;
        }

        public static Faker<ApiResourceClaim> GetApiResourceClaimFaker(int id)
        {
            var fakerApiResourceClaim = new Faker<ApiResourceClaim>()
                .RuleFor(o => o.Type, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.Id, id);

            return fakerApiResourceClaim;
        }

        public static ApiResource GenerateRandomApiResource(int id)
        {
            var apiResource = GetApiResourceFaker(id).Generate();

            return apiResource;
        }

        public static ApiResourceSecret GenerateRandomApiSecret(int id)
        {
            var apiSecret = GetApiSecretFaker(id).Generate();

            return apiSecret;
        }
    }
}
