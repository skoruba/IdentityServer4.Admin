using System;
using System.Collections.Generic;
using Bogus;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Grant;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mocks
{
    public class PersistedGrantDtoMock
    {
        public static List<string> PersistedGransList()
        {
            var persistedGrants = new List<string> { "user_consent", "refresh_token", "reference_token" };

            return persistedGrants;
        }

        public static Faker<PersistedGrantDto> PersistedGrantFaker(string key, int subjectId = 0)
        {
            var persistedGrantFaker = new Faker<PersistedGrantDto>()
                .StrictMode(true)
                .RuleFor(o => o.Key, key)
                .RuleFor(o => o.ClientId, Guid.NewGuid().ToString)
                .RuleFor(o => o.CreationTime, f => f.Date.Past())
                .RuleFor(o => o.Data, f => f.Random.Words(f.Random.Number(1, 10)))
                .RuleFor(o => o.Type, f => f.PickRandom(PersistedGransList()))
                .RuleFor(o => o.Expiration, f => f.Date.Future())
                .RuleFor(o => o.SubjectId, f => subjectId == 0 ? f.Random.Number(int.MaxValue).ToString() : subjectId.ToString());

            return persistedGrantFaker;
        }

        public static PersistedGrantDto GenerateRandomPersistedGrant(string key, int subjectId = 0)
        {
            var persistedGrantFaker = PersistedGrantFaker(key, subjectId);

            var persistedGrant = persistedGrantFaker.Generate();

            return persistedGrant;
        }
    }
}



