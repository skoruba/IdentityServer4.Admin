using Bogus;
using Skoruba.IdentityServer4.Audit.Core.Entities;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mocks
{
    public class AuditEntityMock
    {
        public static Faker<AuditEntity> GetAuditEntityFaker(int id)
        {
            var fakerEntity = new Faker<AuditEntity>()
                .RuleFor(o => o.Id, id)
                .RuleFor(o => o.TimeStamp, f => f.Date.Future())
                .RuleFor(o => o.Source, f => f.Random.Word())
                .RuleFor(o => o.ActorType, f => f.Random.Word())
                .RuleFor(o => o.ActorId, f => f.Random.Guid().ToString())
                .RuleFor(o => o.Actor, f => f.Random.Word())
                .RuleFor(o => o.Action, f => f.Random.Word())
                .RuleFor(o => o.Category, f => f.Random.Word())
                .RuleFor(o => o.SubjectType, f => f.Random.Word())
                .RuleFor(o => o.Subject, f => f.Random.Word())
                .RuleFor(o => o.SubjectId, f => f.Random.Guid().ToString())
                .RuleFor(o => o.EventType, f => f.Random.Word())
                .RuleFor(o => o.Changes, f => f.Random.Words(f.Random.Number(1, 3)))
                .RuleFor(o => o.RemoteIpAddress, f => f.Random.Word())
                .RuleFor(o => o.LocalIpAddress, f => f.Random.Word())
                .RuleFor(o => o.ActorId, f => f.Random.Guid().ToString())
                ;
            return fakerEntity;
        }

        public static AuditEntity GenerateRandomAuditEntity(int id)
        {
            var audit = GetAuditEntityFaker(id).Generate();

            return audit;
        }
    }
}