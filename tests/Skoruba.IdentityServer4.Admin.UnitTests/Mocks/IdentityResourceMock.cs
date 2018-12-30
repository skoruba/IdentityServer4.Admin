using System;
using Bogus;
using IdentityServer4.EntityFramework.Entities;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mocks
{
    public static class IdentityResourceMock
    {
        public static Faker<IdentityResource> GetIdentityResourceFaker(int id)
        {
            var fakerIdentityResource = new Faker<IdentityResource>()
                .RuleFor(o => o.Name, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.Id, id)
                .RuleFor(o => o.Description, f => f.Random.Words(f.Random.Number(1, 5)))
                .RuleFor(o => o.DisplayName, f => f.Random.Words(f.Random.Number(1, 5)))
                .RuleFor(o => o.Enabled, f => f.Random.Bool())
                .RuleFor(o => o.Emphasize, f => f.Random.Bool())
                .RuleFor(o => o.ShowInDiscoveryDocument, f => f.Random.Bool())
                .RuleFor(o => o.Required, f => f.Random.Bool())                
                .RuleFor(o => o.UserClaims, f => GetIdentityClaimFaker(0).Generate(f.Random.Number(10)));

            return fakerIdentityResource;
        }

	    public static IdentityResourceProperty GenerateRandomIdentityResourceProperty(int id)
	    {
		    var identityResourcePropertyFaker = IdentityResourcePropertyFaker(id);

		    var identityResourceProperty = identityResourcePropertyFaker.Generate();

		    return identityResourceProperty;
	    }

	    public static Faker<IdentityResourceProperty> IdentityResourcePropertyFaker(int id)
	    {
		    var identityResourcePropertyFaker = new Faker<IdentityResourceProperty>()
			    .StrictMode(false)
			    .RuleFor(o => o.Id, id)
			    .RuleFor(o => o.Key, f => Guid.NewGuid().ToString())
			    .RuleFor(o => o.Value, f => f.Random.Words(f.Random.Number(1, 5)));

		    return identityResourcePropertyFaker;
	    }

		public static Faker<IdentityClaim> GetIdentityClaimFaker(int id)
        {
            var fakerIdentityClaim = new Faker<IdentityClaim>()
                .RuleFor(o => o.Type, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.Id, id);

            return fakerIdentityClaim;
        }

        public static IdentityResource GenerateRandomIdentityResource(int id)
        {
            var identityResource = GetIdentityResourceFaker(id).Generate();

            return identityResource;
        }
    }
}
