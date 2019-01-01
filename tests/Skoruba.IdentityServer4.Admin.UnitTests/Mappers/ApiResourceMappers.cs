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

			apiResource.ShouldBeEquivalentTo(apiResourceDto, options =>
				options.Excluding(o => o.Secrets)
					   .Excluding(o => o.Scopes)
					   .Excluding(o => o.Properties)
					   .Excluding(o => o.Created)
					   .Excluding(o => o.Updated)
					   .Excluding(o => o.LastAccessed)
					   .Excluding(o => o.NonEditable)
					   .Excluding(o => o.UserClaims));

			//Assert collection
			apiResource.UserClaims.Select(x => x.Type).ShouldBeEquivalentTo(apiResourceDto.UserClaims);
		}

		[Fact]
		public void CanMapApiResourceDtoToEntity()
		{
			//Generate DTO
			var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(1);

			//Try map to entity
			var apiResource = apiResourceDto.ToEntity();

			apiResource.Should().NotBeNull();

			apiResource.ShouldBeEquivalentTo(apiResourceDto, options =>
				options.Excluding(o => o.Secrets)
					.Excluding(o => o.Scopes)
					.Excluding(o => o.Properties)
					.Excluding(o => o.Created)
					.Excluding(o => o.Updated)
					.Excluding(o => o.LastAccessed)
					.Excluding(o => o.NonEditable)
					.Excluding(o => o.UserClaims));

			//Assert collection
			apiResource.UserClaims.Select(x => x.Type).ShouldBeEquivalentTo(apiResourceDto.UserClaims);
		}

		[Fact]
		public void CanMapApiScopeToModel()
		{
			//Generate entity
			var apiResource = ApiResourceMock.GenerateRandomApiResource(1);

			//Try map to DTO
			var apiResourceDto = apiResource.ToModel();

			//Asert
			apiResourceDto.Should().NotBeNull();

			apiResource.ShouldBeEquivalentTo(apiResourceDto, options =>
				options.Excluding(o => o.Secrets)
					.Excluding(o => o.Scopes)
					.Excluding(o => o.Properties)
					.Excluding(o => o.Created)
					.Excluding(o => o.Updated)
					.Excluding(o => o.LastAccessed)
					.Excluding(o => o.NonEditable)
					.Excluding(o => o.UserClaims));

			//Assert collection
			apiResource.UserClaims.Select(x => x.Type).ShouldBeEquivalentTo(apiResourceDto.UserClaims);
		}

		[Fact]
		public void CanMapApiScopeDtoToEntity()
		{
			//Generate DTO
			var apiScopeDto = ApiResourceDtoMock.GenerateRandomApiScope(1, 1);

			//Try map to entity
			var apiScope = apiScopeDto.ToEntity();

			apiScope.Should().NotBeNull();

			apiScope.ShouldBeEquivalentTo(apiScopeDto, options =>
				options.Excluding(o => o.UserClaims)
					   .Excluding(o => o.ApiResource)
					   .Excluding(o => o.Id));

			//Assert collection
			apiScope.UserClaims.Select(x => x.Type).ShouldBeEquivalentTo(apiScopeDto.UserClaims);
			apiScope.Id.Should().Be(apiScopeDto.ApiScopeId);
		}

		[Fact]
		public void CanMapApiSecretToModel()
		{
			//Generate entity
			var apiSecret = ApiResourceMock.GenerateRandomApiSecret(1);

			//Try map to DTO
			var apiSecretsDto = apiSecret.ToModel();

			//Assert
			apiSecretsDto.Should().NotBeNull();

			apiSecret.ShouldBeEquivalentTo(apiSecretsDto, options =>
				options.Excluding(o => o.ApiResource)
					.Excluding(o => o.Created)
					.Excluding(o => o.Id));

			apiSecret.Id.Should().Be(apiSecretsDto.ApiSecretId);
		}

		[Fact]
		public void CanMapApiSecretDtoToEntity()
		{
			//Generate DTO
			var apiSecretsDto = ApiResourceDtoMock.GenerateRandomApiSecret(1, 1);

			//Try map to entity
			var apiSecret = apiSecretsDto.ToEntity();

			apiSecret.Should().NotBeNull();

			apiSecret.ShouldBeEquivalentTo(apiSecretsDto, options =>
				options.Excluding(o => o.ApiResource)
					.Excluding(o => o.Created)
					.Excluding(o => o.Id));

			apiSecret.Id.Should().Be(apiSecretsDto.ApiSecretId);
		}
	}
}
