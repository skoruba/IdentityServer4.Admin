using System.Linq;
using FluentAssertions;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
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

            client.Should().BeEquivalentTo(clientDto, options =>
                options.Excluding(o => o.AccessTokenTypes)
                       .Excluding(o => o.Created)
                       .Excluding(o => o.AllowedCorsOrigins)
                       .Excluding(o => o.AllowedCorsOriginsItems)
                       .Excluding(o => o.AllowedGrantTypes)
                       .Excluding(o => o.AllowedGrantTypesItems)
                       .Excluding(o => o.AllowedScopes)
                       .Excluding(o => o.AllowedScopesItems)
                       .Excluding(o => o.Claims)
                       .Excluding(o => o.ClientSecrets)
                       .Excluding(o => o.ClientType)
                       .Excluding(o => o.Properties)
                       .Excluding(o => o.ProtocolTypes)
                       .Excluding(o => o.PostLogoutRedirectUris)
                       .Excluding(o => o.PostLogoutRedirectUrisItems)
                       .Excluding(o => o.RedirectUris)
                       .Excluding(o => o.RedirectUrisItems)
                       .Excluding(o => o.RefreshTokenUsages)
                       .Excluding(o => o.RefreshTokenExpirations)
                       .Excluding(o => o.IdentityProviderRestrictions)
                       .Excluding(o => o.IdentityProviderRestrictionsItems));

            //Assert collection
            client.AllowedCorsOrigins.Select(x => x.Origin).Should().BeEquivalentTo(clientDto.AllowedCorsOrigins);
            client.RedirectUris.Select(x => x.RedirectUri).Should().BeEquivalentTo(clientDto.RedirectUris);
            client.PostLogoutRedirectUris.Select(x => x.PostLogoutRedirectUri).Should().BeEquivalentTo(clientDto.PostLogoutRedirectUris);
            client.AllowedGrantTypes.Select(x => x.GrantType).Should().BeEquivalentTo(clientDto.AllowedGrantTypes);
            client.AllowedScopes.Select(x => x.Scope).Should().BeEquivalentTo(clientDto.AllowedScopes);
            client.IdentityProviderRestrictions.Select(x => x.Provider).Should().BeEquivalentTo(clientDto.IdentityProviderRestrictions);
        }

        [Fact]
        public void CanMapClientDtoToEntity()
        {
            //Generate DTO
            var clientDto = ClientDtoMock.GenerateRandomClient(0);

            //Try map to entity
            var client = clientDto.ToEntity();

            client.Should().NotBeNull();

            client.Should().BeEquivalentTo(clientDto, options =>
                options.Excluding(o => o.AccessTokenTypes)
                       .Excluding(o => o.Created)
                       .Excluding(o => o.AllowedCorsOrigins)
                       .Excluding(o => o.AllowedCorsOriginsItems)
                       .Excluding(o => o.AllowedGrantTypes)
                       .Excluding(o => o.AllowedGrantTypesItems)
                       .Excluding(o => o.AllowedScopes)
                       .Excluding(o => o.AllowedScopesItems)
                       .Excluding(o => o.Claims)
                       .Excluding(o => o.ClientSecrets)
                       .Excluding(o => o.ClientType)
                       .Excluding(o => o.Properties)
                       .Excluding(o => o.ProtocolTypes)
                       .Excluding(o => o.PostLogoutRedirectUris)
                       .Excluding(o => o.PostLogoutRedirectUrisItems)
                       .Excluding(o => o.RedirectUris)
                       .Excluding(o => o.RedirectUrisItems)
                       .Excluding(o => o.RefreshTokenUsages)
                       .Excluding(o => o.RefreshTokenExpirations)
                       .Excluding(o => o.IdentityProviderRestrictions)
                       .Excluding(o => o.IdentityProviderRestrictionsItems));

            //Assert collection
            client.AllowedCorsOrigins.Select(x => x.Origin).Should().BeEquivalentTo(clientDto.AllowedCorsOrigins);
            client.RedirectUris.Select(x => x.RedirectUri).Should().BeEquivalentTo(clientDto.RedirectUris);
            client.PostLogoutRedirectUris.Select(x => x.PostLogoutRedirectUri).Should().BeEquivalentTo(clientDto.PostLogoutRedirectUris);
            client.AllowedGrantTypes.Select(x => x.GrantType).Should().BeEquivalentTo(clientDto.AllowedGrantTypes);
            client.AllowedScopes.Select(x => x.Scope).Should().BeEquivalentTo(clientDto.AllowedScopes);
            client.IdentityProviderRestrictions.Select(x => x.Provider).Should().BeEquivalentTo(clientDto.IdentityProviderRestrictions);
        }

        [Fact]
        public void CanMapClientClaimToModel()
        {
            var clientClaim = ClientMock.GenerateRandomClientClaim(0);

            var clientClaimDto = clientClaim.ToModel();

            //Assert
            clientClaimDto.Should().NotBeNull();

            clientClaim.Should().BeEquivalentTo(clientClaimDto, options =>
                options.Excluding(o => o.ClientClaims)
                       .Excluding(o => o.ClientClaimId)
                       .Excluding(o => o.ClientName)
                       .Excluding(o => o.PageSize)
                       .Excluding(o => o.TotalCount)
                       .Excluding(o => o.ClientId));
        }

        [Fact]
        public void CanMapClientClaimToEntity()
        {
            var clientClaimDto = ClientDtoMock.GenerateRandomClientClaim(0, 0);

            var clientClaim = clientClaimDto.ToEntity();

            //Assert
            clientClaim.Should().NotBeNull();

            clientClaim.Should().BeEquivalentTo(clientClaimDto, options =>
                options.Excluding(o => o.ClientName)
                       .Excluding(o => o.PageSize)
                       .Excluding(o => o.TotalCount)
                       .Excluding(o => o.ClientClaims)
                       .Excluding(o => o.ClientClaimId)
                       .Excluding(o => o.ClientId));
        }

        [Fact]
        public void CanMapClientSecretToModel()
        {
            var clientSecret = ClientMock.GenerateRandomClientSecret(0);

            var clientSecretsDto = clientSecret.ToModel();

            //Assert
            clientSecretsDto.Should().NotBeNull();

            clientSecret.Should().BeEquivalentTo(clientSecretsDto, options =>
                options.Excluding(o => o.ClientSecretId)
                       .Excluding(o => o.ClientSecrets)
                       .Excluding(o => o.ClientName)
                       .Excluding(o => o.PageSize)
                       .Excluding(o => o.TotalCount)
                       .Excluding(o => o.HashType)
                       .Excluding(o => o.HashTypes)
                       .Excluding(o => o.HashTypeEnum)
                       .Excluding(o => o.TypeList)
                       .Excluding(o => o.ClientId));
        }

        [Fact]
        public void CanMapClientSecretToEntity()
        {
            var clientSecretsDto = ClientDtoMock.GenerateRandomClientSecret(0, 0);

            var clientSecret = clientSecretsDto.ToEntity();

            //Assert
            clientSecret.Should().NotBeNull();

            clientSecret.Should().BeEquivalentTo(clientSecretsDto, options =>
                options.Excluding(o => o.ClientSecretId)
                       .Excluding(o => o.ClientSecrets)
                       .Excluding(o => o.ClientName)
                       .Excluding(o => o.PageSize)
                       .Excluding(o => o.TotalCount)
                       .Excluding(o => o.HashType)
                       .Excluding(o => o.HashTypes)
                       .Excluding(o => o.HashTypeEnum)
                       .Excluding(o => o.TypeList)
                       .Excluding(o => o.ClientId));
        }

        [Fact]
        public void CanMapClientPropertyToModel()
        {
            var clientProperty = ClientMock.GenerateRandomClientProperty(0);

            var clientPropertiesDto = clientProperty.ToModel();

            //Assert
            clientPropertiesDto.Should().NotBeNull();

            clientProperty.Should().BeEquivalentTo(clientPropertiesDto, options =>
                options.Excluding(o => o.ClientName)
                       .Excluding(o => o.PageSize)
                       .Excluding(o => o.TotalCount)
                       .Excluding(o => o.ClientProperties)
                       .Excluding(o => o.ClientPropertyId)
                       .Excluding(o => o.ClientId));
        }

        [Fact]
        public void CanMapClientPropertyToEntity()
        {
            var clientPropertiesDto = ClientDtoMock.GenerateRandomClientProperty(0, 0);

            var clientProperty = clientPropertiesDto.ToEntity();

            //Assert
            clientProperty.Should().NotBeNull();

            clientProperty.Should().BeEquivalentTo(clientPropertiesDto, options =>
                options.Excluding(o => o.ClientName)
                       .Excluding(o => o.PageSize)
                       .Excluding(o => o.TotalCount)
                       .Excluding(o => o.ClientProperties)
                       .Excluding(o => o.ClientPropertyId)
                       .Excluding(o => o.ClientId));
        }
    }
}
