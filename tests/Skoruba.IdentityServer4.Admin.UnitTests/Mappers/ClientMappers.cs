using System.Linq;
using FluentAssertions;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers.Converters;
using Skoruba.IdentityServer4.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mappers
{
    public class ClientMappers
    {
        [Fact]
        public void CanMapClientToModel()
        {
            //Generate entity
            var client = ClientMock.GenerateRandomClient(0);

            //Try map to DTO
            var clientDto = client.ToModel();

            //Asert
            clientDto.Should().NotBeNull();

            clientDto.Should().BeEquivalentTo(client, options =>
                options.Excluding(o => o.AllowedCorsOrigins)
                       .Excluding(o => o.RedirectUris)
                       .Excluding(o => o.PostLogoutRedirectUris)
                       .Excluding(o => o.AllowedGrantTypes)
                       .Excluding(o => o.AllowedScopes)
                       .Excluding(o => o.ClientSecrets)
                       .Excluding(o => o.Created)
                       .Excluding(o => o.AllowedIdentityTokenSigningAlgorithms)
                       .Excluding(o => o.IdentityProviderRestrictions));

            //Assert collection
            client.AllowedCorsOrigins.Select(x => x.Origin).Should().BeEquivalentTo(clientDto.AllowedCorsOrigins);
            client.RedirectUris.Select(x => x.RedirectUri).Should().BeEquivalentTo(clientDto.RedirectUris);
            client.PostLogoutRedirectUris.Select(x => x.PostLogoutRedirectUri).Should().BeEquivalentTo(clientDto.PostLogoutRedirectUris);
            client.AllowedGrantTypes.Select(x => x.GrantType).Should().BeEquivalentTo(clientDto.AllowedGrantTypes);
            client.AllowedScopes.Select(x => x.Scope).Should().BeEquivalentTo(clientDto.AllowedScopes);
            client.IdentityProviderRestrictions.Select(x => x.Provider).Should().BeEquivalentTo(clientDto.IdentityProviderRestrictions);
            var allowedAlgList = AllowedSigningAlgorithmsConverter.Converter.Convert(client.AllowedIdentityTokenSigningAlgorithms, null);
            allowedAlgList.Should().BeEquivalentTo(clientDto.AllowedIdentityTokenSigningAlgorithms);
        }

        [Fact]
        public void CanMapClientDtoToEntity()
        {
            //Generate DTO
            var clientDto = ClientDtoMock.GenerateRandomClient(0);

            //Try map to entity
            var client = clientDto.ToEntity();

            client.Should().NotBeNull();

            clientDto.Should().BeEquivalentTo(client, options =>
                options.Excluding(o => o.AllowedCorsOrigins)
                    .Excluding(o => o.RedirectUris)
                    .Excluding(o => o.PostLogoutRedirectUris)
                    .Excluding(o => o.AllowedGrantTypes)
                    .Excluding(o => o.AllowedScopes)
                    .Excluding(o => o.ClientSecrets)
                    .Excluding(o => o.AllowedIdentityTokenSigningAlgorithms)
                    .Excluding(o => o.Created)
					.Excluding(o => o.IdentityProviderRestrictions));

            //Assert collection
            client.AllowedCorsOrigins.Select(x => x.Origin).Should().BeEquivalentTo(clientDto.AllowedCorsOrigins);
            client.RedirectUris.Select(x => x.RedirectUri).Should().BeEquivalentTo(clientDto.RedirectUris);
            client.PostLogoutRedirectUris.Select(x => x.PostLogoutRedirectUri).Should().BeEquivalentTo(clientDto.PostLogoutRedirectUris);
            client.AllowedGrantTypes.Select(x => x.GrantType).Should().BeEquivalentTo(clientDto.AllowedGrantTypes);
            client.AllowedScopes.Select(x => x.Scope).Should().BeEquivalentTo(clientDto.AllowedScopes);
            client.IdentityProviderRestrictions.Select(x => x.Provider).Should().BeEquivalentTo(clientDto.IdentityProviderRestrictions);
            var allowedAlgList = AllowedSigningAlgorithmsConverter.Converter.Convert(client.AllowedIdentityTokenSigningAlgorithms, null);
            allowedAlgList.Should().BeEquivalentTo(clientDto.AllowedIdentityTokenSigningAlgorithms);
        }

        [Fact]
        public void CanMapClientClaimToModel()
        {
            var clientClaim = ClientMock.GenerateRandomClientClaim(0);

            var clientClaimsDto = clientClaim.ToModel();

            //Assert
            clientClaimsDto.Should().NotBeNull();

            clientClaimsDto.Should().BeEquivalentTo(clientClaim, options =>
                options.Excluding(o => o.Id)
                    .Excluding(o => o.Client));
        }

        [Fact]
        public void CanMapClientClaimToEntity()
        {
            var clientClaimDto = ClientDtoMock.GenerateRandomClientClaim(0, 0);

            var clientClaim = clientClaimDto.ToEntity();

            //Assert
            clientClaim.Should().NotBeNull();

            clientClaimDto.Should().BeEquivalentTo(clientClaim, options =>
                options.Excluding(o => o.Id)
                    .Excluding(o => o.Client));
        }

        [Fact]
        public void CanMapClientSecretToModel()
        {
            var clientSecret = ClientMock.GenerateRandomClientSecret(0);

            var clientSecretsDto = clientSecret.ToModel();

            //Assert
            clientSecretsDto.Should().NotBeNull();

            clientSecretsDto.Should().BeEquivalentTo(clientSecret, options =>
                options.Excluding(o => o.Id)
	                .Excluding(o => o.Created)
					.Excluding(o => o.Client));
        }

        [Fact]
        public void CanMapClientSecretToEntity()
        {
            var clientSecretsDto = ClientDtoMock.GenerateRandomClientSecret(0, 0);

            var clientSecret = clientSecretsDto.ToEntity();

            //Assert
            clientSecret.Should().NotBeNull();

            clientSecretsDto.Should().BeEquivalentTo(clientSecret, options =>
                options.Excluding(o => o.Id)
	                .Excluding(o => o.Created)
					.Excluding(o => o.Client));
        }

        [Fact]
        public void CanMapClientPropertyToModel()
        {
            var clientProperty = ClientMock.GenerateRandomClientProperty(0);

            var clientPropertiesDto = clientProperty.ToModel();

            //Assert
            clientPropertiesDto.Should().NotBeNull();

            clientPropertiesDto.Should().BeEquivalentTo(clientProperty, options =>
                options.Excluding(o => o.Id)
                    .Excluding(o => o.Client));
        }

        [Fact]
        public void CanMapClientPropertyToEntity()
        {
            var clientPropertiesDto = ClientDtoMock.GenerateRandomClientProperty(0, 0);

            var clientProperty = clientPropertiesDto.ToEntity();

            //Assert
            clientProperty.Should().NotBeNull();

            clientPropertiesDto.Should().BeEquivalentTo(clientProperty, options =>
                options.Excluding(o => o.Id)
                    .Excluding(o => o.Client));
        }
    }
}
