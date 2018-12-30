using System.Linq;
using FluentAssertions;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mappers
{
    public class IdentityResourceMappers
    {
        [Fact]
        public void CanMapIdentityResourceToModel()
        {
            //Generate entity
            var identityResource = IdentityResourceMock.GenerateRandomIdentityResource(1);

            //Try map to DTO
            var identityResourceDto = identityResource.ToModel();

            //Assert
            identityResourceDto.Should().NotBeNull();

            identityResource.ShouldBeEquivalentTo(identityResourceDto, options =>
                options.Excluding(o => o.UserClaims)
		            .Excluding(o => o.Properties)
		            .Excluding(o => o.Created)
		            .Excluding(o => o.Updated)
		            .Excluding(o => o.NonEditable));

            //Assert collection
            identityResource.UserClaims.Select(x => x.Type).ShouldBeEquivalentTo(identityResourceDto.UserClaims);
        }

        [Fact]
        public void CanMapIdentityResourceDtoToEntity()
        {
            //Generate DTO
            var identityResourceDto = IdentityResourceDtoMock.GenerateRandomIdentityResource(1);

            //Try map to entity
            var identityResource = identityResourceDto.ToEntity();

            identityResource.Should().NotBeNull();

            identityResource.ShouldBeEquivalentTo(identityResourceDto, options =>
                options.Excluding(o => o.UserClaims)
				.Excluding(o => o.Properties)
		            .Excluding(o => o.Created)
		            .Excluding(o => o.Updated)
		            .Excluding(o => o.NonEditable));

            //Assert collection
            identityResource.UserClaims.Select(x => x.Type).ShouldBeEquivalentTo(identityResourceDto.UserClaims);
        }
    }
}
