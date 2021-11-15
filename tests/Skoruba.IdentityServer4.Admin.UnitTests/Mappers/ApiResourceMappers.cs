using System.Linq;
using FluentAssertions;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers.Converters;
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

			apiResourceDto.Should().BeEquivalentTo(apiResource, options =>
				options.Excluding(o => o.Secrets)
					   .Excluding(o => o.Scopes)
					   .Excluding(o => o.Properties)
					   .Excluding(o => o.Created)
					   .Excluding(o => o.Updated)
					   .Excluding(o => o.LastAccessed)
					   .Excluding(o => o.NonEditable)
                       .Excluding(o => o.AllowedAccessTokenSigningAlgorithms)
					   .Excluding(o => o.UserClaims));

			//Assert collection
			apiResource.UserClaims.Select(x => x.Type).Should().BeEquivalentTo(apiResourceDto.UserClaims);

            var allowedAlgList = AllowedSigningAlgorithmsConverter.Converter.Convert(apiResource.AllowedAccessTokenSigningAlgorithms, null);
			allowedAlgList.Should().BeEquivalentTo(apiResourceDto.AllowedAccessTokenSigningAlgorithms);
		}

		[Fact]
		public void CanMapApiResourceDtoToEntity()
		{
			//Generate DTO
			var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(1);

			//Try map to entity
			var apiResource = apiResourceDto.ToEntity();

			apiResource.Should().NotBeNull();

			apiResourceDto.Should().BeEquivalentTo(apiResource, options =>
				options.Excluding(o => o.Secrets)
					.Excluding(o => o.Scopes)
					.Excluding(o => o.Properties)
					.Excluding(o => o.Created)
					.Excluding(o => o.Updated)
					.Excluding(o => o.LastAccessed)
					.Excluding(o => o.NonEditable)
                    .Excluding(o => o.AllowedAccessTokenSigningAlgorithms)
					.Excluding(o => o.UserClaims));

			//Assert collection
			apiResource.UserClaims.Select(x => x.Type).Should().BeEquivalentTo(apiResourceDto.UserClaims);
            var allowedAlgList = AllowedSigningAlgorithmsConverter.Converter.Convert(apiResource.AllowedAccessTokenSigningAlgorithms, null);
            allowedAlgList.Should().BeEquivalentTo(apiResourceDto.AllowedAccessTokenSigningAlgorithms);
		}

		[Fact]
		public void CanMapApiScopeToModel()
		{
            //Generate DTO
            var apiScopeDto = ApiScopeMock.GenerateRandomApiScope(1);

			//Try map to entity
			var apiScope = apiScopeDto.ToModel();

            apiScope.Should().NotBeNull();

            apiScopeDto.Should().BeEquivalentTo(apiScope, options =>
                options.Excluding(o => o.UserClaims)
                    .Excluding(o => o.ApiScopeProperties)
                    .Excluding(o => o.UserClaimsItems));

			//Assert collection
            apiScopeDto.UserClaims.Select(x => x.Type).Should().BeEquivalentTo(apiScope.UserClaims);
            apiScope.Id.Should().Be(apiScopeDto.Id);
		}

		[Fact]
		public void CanMapApiScopeDtoToEntity()
		{
			//Generate DTO
			var apiScopeDto = ApiScopeDtoMock.GenerateRandomApiScope(1);

			//Try map to entity
			var apiScope = apiScopeDto.ToEntity();

			apiScope.Should().NotBeNull();

			apiScopeDto.Should().BeEquivalentTo(apiScope, options =>
				options.Excluding(o => o.UserClaims)
                       .Excluding(o => o.Properties)
					   .Excluding(o => o.Id));

			//Assert collection
			apiScope.UserClaims.Select(x => x.Type).Should().BeEquivalentTo(apiScopeDto.UserClaims);
			apiScope.Id.Should().Be(apiScopeDto.Id);
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

			apiSecretsDto.Should().BeEquivalentTo(apiSecret, options =>
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

			apiSecretsDto.Should().BeEquivalentTo(apiSecret, options =>
				options.Excluding(o => o.ApiResource)
					.Excluding(o => o.Created)
					.Excluding(o => o.Id));

			apiSecret.Id.Should().Be(apiSecretsDto.ApiSecretId);
		}
	}
}
