using System.Linq;
using AutoMapper;
using FluentAssertions;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Skoruba.IdentityServer4.Audit.Core.Dtos;
using Skoruba.IdentityServer4.Audit.Core.Mappers;
using Xunit;

namespace Skoruba.IdentityServer4.Audit.UnitTests.Mappers
{
    public class AuditMappers
    {
        [Fact]
        public void CanMapAuditEntityToAuditDto()
        {
            //Register mappers
            Mapper.Initialize(cfg => cfg.AddProfile<AuditMappersProfile>());

            //Generate entity
            var auditEntity = AuditEntityMock.GenerateRandomAuditEntity(1);

            //Try map to DTO
            var auditEntityDto = Mapper.Map<AuditDto>(auditEntity);

            //Assert
            auditEntityDto.Should().NotBeNull();
            auditEntity.ShouldBeEquivalentTo(auditEntityDto);
        }
    }
}