using FluentAssertions;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mappers
{
    public class LogMappers
    {
        [Fact]
        public void CanMapIdentityResourceToModel()
        {
            //Generate entity
            var log = LogMock.GenerateRandomLog(1);

            //Try map to DTO
            var logDto = log.ToModel();

            //Asert
            logDto.Should().NotBeNull();

            log.ShouldBeEquivalentTo(logDto, options =>
                options.Excluding(o => o.PropertiesXml));
        }

        [Fact]
        public void CanMapIdentityResourceDtoToEntity()
        {
            //Generate DTO
            var logDto = LogDtoMock.GenerateRandomLog(1);

            //Try map to entity
            var log = logDto.ToEntity();

            log.Should().NotBeNull();

            log.ShouldBeEquivalentTo(logDto, options =>
                options.Excluding(o => o.PropertiesXml));
        }
    }
}
