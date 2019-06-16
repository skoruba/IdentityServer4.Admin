using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.EntityFramework.Constants;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mocks
{
	public static class ClientDtoMock
	{
		public static ClientDto GenerateRandomClient(int id)
		{
			var clientFaker = ClientFaker(id);

			var clientTesting = clientFaker.Generate();

			return clientTesting;
		}

		public static ClientClaimsDto GenerateRandomClientClaim(int id, int clientId)
		{
			var clientClaimFaker = ClientClaimFaker(id, clientId);

			var clientClaimTesting = clientClaimFaker.Generate();

			return clientClaimTesting;
		}

		public static ClientPropertiesDto GenerateRandomClientProperty(int id, int clientId)
		{
			var clientPropertyFaker = ClientPropertyFaker(id, clientId);

			var clientPropertyTesting = clientPropertyFaker.Generate();

			return clientPropertyTesting;
		}

		public static ClientCloneDto GenerateClientCloneDto(int id, bool cloneClientClaims = true,
			bool cloneClientCorsOrigins = true, bool cloneClientGrantTypes = true, bool cloneClientIdPRestrictions = true,
			bool cloneClientPostLogoutRedirectUris = true, bool cloneClientProperties = true,
			bool cloneClientRedirectUris = true, bool cloneClientScopes = true)
		{
			var clientCloneFaker = ClientCloneFaker(id, cloneClientClaims, cloneClientCorsOrigins,
				cloneClientGrantTypes, cloneClientIdPRestrictions, cloneClientPostLogoutRedirectUris,
				cloneClientProperties, cloneClientRedirectUris, cloneClientScopes);

			var clientCloneDto = clientCloneFaker.Generate();

			return clientCloneDto;
		}

		public static ClientSecretsDto GenerateRandomClientSecret(int id, int clientId)
		{
			var clientSecretFaker = ClientSecretFaker(id, clientId);

			var clientSecretTesting = clientSecretFaker.Generate();

			return clientSecretTesting;
		}

		public static Faker<ClientDto> ClientFaker(int id)
		{
			var clientFaker = new Faker<ClientDto>()
			   .StrictMode(false)
			   .RuleFor(o => o.ClientId, f => Guid.NewGuid().ToString())
			   .RuleFor(o => o.ClientName, f => Guid.NewGuid().ToString())
			   .RuleFor(o => o.Id, id)
			   .RuleFor(o => o.AbsoluteRefreshTokenLifetime, f => f.Random.Number(int.MaxValue))
			   .RuleFor(o => o.AccessTokenLifetime, f => f.Random.Number(int.MaxValue))
			   .RuleFor(o => o.AccessTokenType, f => f.Random.Number(0, 1))
			   .RuleFor(o => o.AllowAccessTokensViaBrowser, f => f.Random.Bool())
			   .RuleFor(o => o.AllowOfflineAccess, f => f.Random.Bool())
			   .RuleFor(o => o.AllowPlainTextPkce, f => f.Random.Bool())
			   .RuleFor(o => o.AllowRememberConsent, f => f.Random.Bool())
			   .RuleFor(o => o.AllowedCorsOrigins, f => Enumerable.Range(1, f.Random.Int(1, 10)).Select(x => f.PickRandom(f.Internet.Url())).ToList())
			   .RuleFor(o => o.AllowedGrantTypes, f => Enumerable.Range(1, f.Random.Int(1, 10)).Select(x => f.PickRandom(ClientConsts.GetGrantTypes())).ToList())
			   .RuleFor(o => o.AllowedScopes, f => Enumerable.Range(1, f.Random.Int(1, 10)).Select(x => f.PickRandom(ClientMock.GetScopes())).ToList())
			   .RuleFor(o => o.AlwaysIncludeUserClaimsInIdToken, f => f.Random.Bool())
			   .RuleFor(o => o.Enabled, f => f.Random.Bool())
			   .RuleFor(o => o.ProtocolType, f => f.PickRandom(ClientConsts.GetProtocolTypes().Select(x => x.Id)))
			   .RuleFor(o => o.ClientSecrets, f => new List<ClientSecretDto>()) //Client Secrets are managed with seperate method
			   .RuleFor(o => o.RequireClientSecret, f => f.Random.Bool())
			   .RuleFor(o => o.Description, f => f.Random.Words(f.Random.Number(1, 7)))
			   .RuleFor(o => o.ClientUri, f => f.Internet.Url())
			   .RuleFor(o => o.RequireConsent, f => f.Random.Bool())
			   .RuleFor(o => o.RequirePkce, f => f.Random.Bool())
			   .RuleFor(o => o.RedirectUris, f => Enumerable.Range(1, f.Random.Int(1, 10)).Select(x => f.PickRandom(f.Internet.Url())).ToList())
			   .RuleFor(o => o.PostLogoutRedirectUris, f => Enumerable.Range(1, f.Random.Int(1, 10)).Select(x => f.PickRandom(f.Internet.Url())).ToList())
			   .RuleFor(o => o.FrontChannelLogoutUri, f => f.Internet.Url())
			   .RuleFor(o => o.FrontChannelLogoutSessionRequired, f => f.Random.Bool())
			   .RuleFor(o => o.BackChannelLogoutUri, f => f.Internet.Url())
			   .RuleFor(o => o.BackChannelLogoutSessionRequired, f => f.Random.Bool())
			   .RuleFor(o => o.IdentityTokenLifetime, f => f.Random.Number(int.MaxValue))
			   .RuleFor(o => o.AuthorizationCodeLifetime, f => f.Random.Number(int.MaxValue))
			   .RuleFor(o => o.ConsentLifetime, f => f.Random.Number(int.MaxValue))
			   .RuleFor(o => o.SlidingRefreshTokenLifetime, f => f.Random.Number(int.MaxValue))
			   .RuleFor(o => o.RefreshTokenUsage, f => f.Random.Number(0, 1))
			   .RuleFor(o => o.UpdateAccessTokenClaimsOnRefresh, f => f.Random.Bool())
			   .RuleFor(o => o.RefreshTokenExpiration, f => f.Random.Number(int.MaxValue))
			   .RuleFor(o => o.EnableLocalLogin, f => f.Random.Bool())
			   .RuleFor(o => o.AlwaysSendClientClaims, f => f.Random.Bool())
			   .RuleFor(o => o.ClientClaimsPrefix, f => Guid.NewGuid().ToString())
			   .RuleFor(o => o.IncludeJwtId, f => f.Random.Bool())
			   .RuleFor(o => o.PairWiseSubjectSalt, f => Guid.NewGuid().ToString())
			   .RuleFor(o => o.Claims, f => new List<ClientClaimDto>()) //Client Claims are managed with seperate method
			   .RuleFor(o => o.IdentityProviderRestrictions, f => Enumerable.Range(1, f.Random.Int(1, 10)).Select(x => f.PickRandom(ClientMock.GetIdentityProviders())).ToList())
			   .RuleFor(o => o.Properties, f => new List<ClientPropertyDto>()) //Client Properties are managed with seperate method
			   .RuleFor(o => o.LogoUri, f => f.Internet.Url())
			   .RuleFor(o => o.Updated, f => f.Date.Recent())
			   .RuleFor(o => o.LastAccessed, f => f.Date.Recent())
			   .RuleFor(o => o.UserSsoLifetime, f => f.Random.Int())
			   .RuleFor(o => o.UserCodeType, f => f.Random.Word())
			   .RuleFor(o => o.DeviceCodeLifetime, f => f.Random.Int())
		       .RuleFor(o => o.NonEditable, f => f.Random.Bool());

			return clientFaker;
		}

		public static Faker<ClientClaimsDto> ClientClaimFaker(int id, int clientId)
		{
			var clientClaimFaker = new Faker<ClientClaimsDto>()
				.StrictMode(false)
				.RuleFor(o => o.ClientClaimId, id)
				.RuleFor(o => o.Type, f => f.PickRandom(ClientConsts.GetStandardClaims()))
				.RuleFor(o => o.Value, f => Guid.NewGuid().ToString())
				.RuleFor(o => o.ClientId, clientId);

			return clientClaimFaker;
		}

		public static Faker<ClientSecretsDto> ClientSecretFaker(int id, int clientId)
		{
			var clientSecretFaker = new Faker<ClientSecretsDto>()
				.StrictMode(false)
				.RuleFor(o => o.ClientSecretId, id)
				.RuleFor(o => o.Type, f => f.PickRandom(ClientConsts.GetSecretTypes()))
				.RuleFor(o => o.Value, f => Guid.NewGuid().ToString())
				.RuleFor(o => o.ClientId, clientId);

			return clientSecretFaker;
		}

		public static Faker<ClientPropertiesDto> ClientPropertyFaker(int id, int clientId)
		{
			var clientPropertyFaker = new Faker<ClientPropertiesDto>()
				.StrictMode(false)
				.RuleFor(o => o.ClientPropertyId, id)
				.RuleFor(o => o.Key, f => Guid.NewGuid().ToString())
				.RuleFor(o => o.Value, f => Guid.NewGuid().ToString())
				.RuleFor(o => o.ClientId, clientId);

			return clientPropertyFaker;
		}

		public static Faker<ClientCloneDto> ClientCloneFaker(int id, bool cloneClientClaims,
		bool cloneClientCorsOrigins, bool cloneClientGrantTypes, bool cloneClientIdPRestrictions,
		bool cloneClientPostLogoutRedirectUris, bool cloneClientProperties,
		bool cloneClientRedirectUris, bool cloneClientScopes)
		{
			var clientCloneDto = new Faker<ClientCloneDto>()
				.StrictMode(false)
				.RuleFor(o => o.Id, id)
				.RuleFor(o => o.CloneClientClaims, cloneClientClaims)
				.RuleFor(o => o.CloneClientCorsOrigins, cloneClientCorsOrigins)
				.RuleFor(o => o.CloneClientGrantTypes, cloneClientGrantTypes)
				.RuleFor(o => o.CloneClientIdPRestrictions, cloneClientIdPRestrictions)
				.RuleFor(o => o.CloneClientPostLogoutRedirectUris, cloneClientPostLogoutRedirectUris)
				.RuleFor(o => o.CloneClientProperties, cloneClientProperties)
				.RuleFor(o => o.CloneClientRedirectUris, cloneClientRedirectUris)
				.RuleFor(o => o.CloneClientScopes, cloneClientScopes);

			return clientCloneDto;
		}
	}
}
