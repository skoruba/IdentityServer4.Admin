using System.Linq;
using FluentAssertions;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mappers
{
	public class ApiResourceMappers
	{
		[Fact]
		public void CanMapApiResourceToModel()
		{
			//Generate entity
			var apiResource = ApiResourceMock.GenerateRandomApiResource(1);

			//Try map to DTO
			var apiResourceDto = apiResource.ToModel();

			//Assert
			apiResourceDto.Should().NotBeNull();

			apiResource.Should().BeEquivalentTo(apiResourceDto, options => 
						options.Excluding(o => o.UserClaims)
							   .Excluding(o => o.UserClaimsItems));

			//Assert collection
			apiResource.UserClaims.Select(x => x.Type).Should().BeEquivalentTo(apiResourceDto.UserClaims);
		}

		[Fact]
		public void CanMapApiResourceDtoToEntity()
		{
			//Generate DTO
			var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(1);

			//Try map to entity
			var apiResource = apiResourceDto.ToEntity();

			apiResource.Should().NotBeNull();

			apiResource.Should().BeEquivalentTo(apiResourceDto, options =>
				options.Excluding(o => o.UserClaims)
					   .Excluding(o => o.UserClaimsItems));

			//Assert collection
			apiResource.UserClaims.Select(x => x.Type).Should().BeEquivalentTo(apiResourceDto.UserClaims);
		}

		[Fact]
		public void CanMapApiScopeDtoToEntity()
		{
			//Generate DTO
			var apiScopeDto = ApiResourceDtoMock.GenerateRandomApiScope(1, 1);

			//Try map to entity
			var apiScope = apiScopeDto.ToEntity();

			apiScope.Should().NotBeNull();

			apiScope.Should().BeEquivalentTo(apiScopeDto, options =>
				options.Excluding(o => o.UserClaims)
					   .Excluding(o => o.Scopes)
					   .Excluding(o => o.UserClaimsItems)
					   .Excluding(o => o.TotalCount)
					   .Excluding(o => o.PageSize)
					   .Excluding(o => o.ResourceName)
					   .Excluding(o => o.ApiResourceId)
					   .Excluding(o => o.ApiScopeId));

			//Assert collection
			apiScope.UserClaims.Select(x => x.Type).Should().BeEquivalentTo(apiScopeDto.UserClaims);
			apiScope.Id.Should().Be(apiScopeDto.ApiScopeId);
		}

		[Fact]
		public void CanMapApiSecretToModel()
		{
			//Generate entity
			var apiSecret = ApiResourceMock.GenerateRandomApiSecret(1);

			//Try map to DTO
			var apiSecretDto = apiSecret.ToModel();

			//Assert
			apiSecretDto.Should().NotBeNull();

			apiSecret.Should().BeEquivalentTo(apiSecretDto, options =>
				options.Excluding(o => o.Created)
					   .Excluding(o => o.ApiSecretId)
					   .Excluding(o => o.ApiResourceName)
					   .Excluding(o => o.TypeList)
					   .Excluding(o => o.HashType)
					   .Excluding(o => o.HashTypes)
					   .Excluding(o => o.ApiResourceSecrets)
					   .Excluding(o => o.TotalCount)
					   .Excluding(o => o.PageSize));

			apiSecret.Id.Should().Be(apiSecretDto.ApiSecretId);
		}

		[Fact]
		public void CanMapApiSecretDtoToEntity()
		{
			//Generate DTO
			var apiSecretsDto = ApiResourceDtoMock.GenerateRandomApiResourceSecret(1, 1);

			//Try map to entity
			var apiSecret = apiSecretsDto.ToEntity();

			apiSecret.Should().NotBeNull();

			apiSecret.Should().BeEquivalentTo(apiSecretsDto, options =>
				options.Excluding(o => o.Created)
					   .Excluding(o => o.ApiSecretId)
					   .Excluding(o => o.ApiResourceName)
					   .Excluding(o => o.TypeList)
					   .Excluding(o => o.HashType)
					   .Excluding(o => o.HashTypes)
					   .Excluding(o => o.ApiResourceSecrets)
					   .Excluding(o => o.TotalCount)
					   .Excluding(o => o.PageSize));

			apiSecret.Id.Should().Be(apiSecretsDto.ApiSecretId);
		}
	}
}
