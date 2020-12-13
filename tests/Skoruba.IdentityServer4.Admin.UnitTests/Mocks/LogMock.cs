﻿using Bogus;

using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mocks
{
    public class LogMock
    {
        public static Faker<Log> GetLogFaker(int id)
        {
            var fakerLogDto = new Faker<Log>()
                .RuleFor(o => o.Id, id)
                .RuleFor(o => o.Exception, f => f.Random.Words(f.Random.Number(1, 30)))
                .RuleFor(o => o.LogEvent, f => f.Random.Words(f.Random.Number(1, 30)))
                .RuleFor(o => o.Level, f => f.Random.Words(1))
                .RuleFor(o => o.TimeStamp, f => f.Date.Future())
                .RuleFor(o => o.Message, f => f.Random.Words(f.Random.Number(1, 30)))
                .RuleFor(o => o.MessageTemplate, f => f.Random.Words(f.Random.Number(1, 30)))
                .RuleFor(o => o.Properties, f => f.Random.Words(f.Random.Number(1, 30)));

            return fakerLogDto;
        }

        public static Log GenerateRandomLog(int id)
        {
            var log = GetLogFaker(id).Generate();

            return log;
        }
    }
}
