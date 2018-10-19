using System;
using System.Collections.Generic;
using Bogus;
using IdentityServer4.EntityFramework.Entities;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mocks
{
    public class PersistedGrantMock
    {
        public static List<string> PersistedGransList()
        {
            var persistedGrants = new List<string> { "user_consent", "refresh_token", "reference_token" };

            return persistedGrants;
        }

        public static Faker<PersistedGrant> PersistedGrantFaker(string key, string subjectId = null)
        {
            var persistedGrantFaker = new Faker<PersistedGrant>()
                .StrictMode(true)
                .RuleFor(o => o.Key, key)
                .RuleFor(o => o.ClientId, Guid.NewGuid().ToString)
                .RuleFor(o => o.CreationTime, f => f.Date.Past())
                .RuleFor(o => o.Data, f => f.Random.Words(f.Random.Number(1, 10)))
                .RuleFor(o => o.Type, f => f.PickRandom(PersistedGransList()))
                .RuleFor(o => o.Expiration, f => f.Date.Future())
                .RuleFor(o => o.SubjectId, f => string.IsNullOrEmpty(subjectId) ? f.Random.Number(int.MaxValue).ToString() : subjectId.ToString());

            return persistedGrantFaker;
        }

        public static PersistedGrant GenerateRandomPersistedGrant(string key, string subjectId = null)
        {
            var persistedGrantFaker = PersistedGrantFaker(key, subjectId);

            var persistedGrant = persistedGrantFaker.Generate();

            return persistedGrant;
        }
    }
}



