using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Constants;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mocks
{
	public static class ClientMock
	{
		public static List<string> GetScopes()
		{
			var scopes = new List<string>
			{
				"openid",
				"profile",
				"email"
			};

			return scopes;
		}

		public static List<string> GetIdentityProviders()
		{
			var providers = new List<string>
			{
				"facebook",
				"google"
			};

			return providers;
		}

		public static Faker<Client> ClientFaker(int id, bool generateClaims = false, bool generateProperties = false, bool generateSecrets = false)
		{
			var clientFaker = new Faker<Client>()
				.StrictMode(true)
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
				.RuleFor(o => o.AllowedCorsOrigins, f => GetClientCorsOriginFaker().Generate(f.Random.Number(10)))
				.RuleFor(o => o.AllowedGrantTypes, f => ClientGrantTypesFaker().Generate(1))
				.RuleFor(o => o.AllowedScopes, f => ClientScopesFaker().Generate(f.Random.Number(1, 3)))
				.RuleFor(o => o.AlwaysIncludeUserClaimsInIdToken, f => f.Random.Bool())
				.RuleFor(o => o.Enabled, f => f.Random.Bool())
				.RuleFor(o => o.ProtocolType, f => f.PickRandom(ClientConsts.GetProtocolTypes().Select(x => x.Id)))
				.RuleFor(o => o.ClientSecrets,
					f => generateSecrets
						? ClientSecretFaker(0).Generate(f.Random.Number(10))
						: new List<ClientSecret>()) //Client Secrets are managed with separate method
				.RuleFor(o => o.RequireClientSecret, f => f.Random.Bool())
				.RuleFor(o => o.Description, f => f.Random.Words(f.Random.Number(1, 7)))
				.RuleFor(o => o.ClientUri, f => f.Internet.Url())
				.RuleFor(o => o.RequireConsent, f => f.Random.Bool())
				.RuleFor(o => o.RequirePkce, f => f.Random.Bool())
				.RuleFor(o => o.RedirectUris, f => ClientRedirectUriFaker().Generate(f.Random.Number(10)))
				.RuleFor(o => o.PostLogoutRedirectUris,
					f => ClientPostLogoutRedirectUriFaker().Generate(f.Random.Number(10)))
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
				.RuleFor(o => o.Claims,
					f => generateClaims
						? ClientClaimFaker(0).Generate(f.Random.Number(10))
						: new List<ClientClaim>()) //Client Claims are managed with separate method
				.RuleFor(o => o.IdentityProviderRestrictions, f => ClientIdPRescrictionFaker().Generate(1))
				.RuleFor(o => o.Properties,
					f => generateProperties
						? ClientPropertyFaker(0).Generate(f.Random.Number(10))
						: new List<ClientProperty>()) //Client Properties are managed with separate method
				.RuleFor(o => o.LogoUri, f => f.Internet.Url())
				.RuleFor(o => o.Created, f => f.Date.Recent())
				.RuleFor(o => o.Updated, f => f.Date.Recent())
				.RuleFor(o => o.LastAccessed, f => f.Date.Recent())
				.RuleFor(o => o.UserSsoLifetime, f => f.Random.Int())
				.RuleFor(o => o.UserCodeType, f => f.Random.Word())
				.RuleFor(o => o.DeviceCodeLifetime, f => f.Random.Int())
				.RuleFor(o => o.NonEditable, f => f.Random.Bool());

			return clientFaker;
		}

		public static Faker<ClientCorsOrigin> GetClientCorsOriginFaker()
		{
			var fakerClientCorsOrigin = new Faker<ClientCorsOrigin>()
				.RuleFor(o => o.Origin, f => f.Internet.Url());

			return fakerClientCorsOrigin;
		}

		public static Client GenerateRandomClient(int id, bool generateClaims = false, bool generateProperties = false, bool generateSecrets = false)
		{
			var clientFaker = ClientFaker(id, generateClaims, generateProperties, generateSecrets);

			var clientTesting = clientFaker.Generate();

			return clientTesting;
		}

		public static Client GenerateRandomClientToClone(int id)
		{
			var clientFaker = ClientFaker(id);

			var clientTesting = clientFaker.Generate();

			return clientTesting;
		}

		public static List<Client> GenerateRandomClients(int id, int clientCount)
		{
			var clientFaker = ClientFaker(id);

			var clientTesting = clientFaker.Generate(clientCount).ToList();

			return clientTesting;
		}

		public static Faker<ClientIdPRestriction> ClientIdPRescrictionFaker()
		{
			var fakerClientIdPRescriction = new Faker<ClientIdPRestriction>()
				.RuleFor(o => o.Provider, f => f.PickRandom(GetIdentityProviders()));
			return fakerClientIdPRescriction;
		}

		public static Faker<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUriFaker()
		{
			var fakerClientPostLogoutRedirectUri = new Faker<ClientPostLogoutRedirectUri>()
				.RuleFor(o => o.PostLogoutRedirectUri, f => f.Internet.Url());
			return fakerClientPostLogoutRedirectUri;
		}

		public static Faker<ClientRedirectUri> ClientRedirectUriFaker()
		{
			var fakerClientRedirectUri = new Faker<ClientRedirectUri>()
				.RuleFor(o => o.RedirectUri, f => f.Internet.Url());
			return fakerClientRedirectUri;
		}

		public static Faker<ClientScope> ClientScopesFaker()
		{
			var fakerClientScopes = new Faker<ClientScope>()
				.RuleFor(o => o.Scope, f => f.PickRandom(GetScopes()));
			return fakerClientScopes;
		}

		public static Faker<ClientGrantType> ClientGrantTypesFaker()
		{
			var fakerClientGrantTypes = new Faker<ClientGrantType>()
				.RuleFor(o => o.GrantType, f => f.PickRandom(ClientConsts.GetGrantTypes()));

			return fakerClientGrantTypes;
		}

		public static ClientGrantType GenerateRandomClientGrantType()
		{
			var clientGrantType = ClientGrantTypesFaker().Generate();

			return clientGrantType;
		}

		public static ClientSecret GenerateRandomClientSecret(int id)
		{
			var testClientSecret = ClientSecretFaker(id);

			var clientSecretTesting = testClientSecret.Generate();

			return clientSecretTesting;
		}

		public static ClientClaim GenerateRandomClientClaim(int id)
		{
			var clientClaimFaker = ClientClaimFaker(id);

			var clientClaimTesting = clientClaimFaker.Generate();

			return clientClaimTesting;
		}

		public static ClientProperty GenerateRandomClientProperty(int id)
		{
			var clientPropertyFaker = ClientPropertyFaker(id);

			var clientPropertyTesting = clientPropertyFaker.Generate();

			return clientPropertyTesting;
		}

		public static Faker<ClientSecret> ClientSecretFaker(int id)
		{
			var testClientSecret = new Faker<ClientSecret>()
				.StrictMode(false)
				.RuleFor(o => o.Id, id)
				.RuleFor(o => o.Description, f => f.Random.Words(f.Random.Number(1, 7)))
				.RuleFor(o => o.Type, f => f.PickRandom(ClientConsts.GetSecretTypes()))
				.RuleFor(o => o.Expiration, f => f.Date.Future())
				.RuleFor(o => o.Value, f => Guid.NewGuid().ToString());

			return testClientSecret;
		}

		public static Faker<ClientClaim> ClientClaimFaker(int id)
		{
			var clientClaimFaker = new Faker<ClientClaim>()
				.StrictMode(false)
				.RuleFor(o => o.Id, id)
				.RuleFor(o => o.Type, f => f.PickRandom(ClientConsts.GetStandardClaims()))
				.RuleFor(o => o.Value, f => Guid.NewGuid().ToString());

			return clientClaimFaker;
		}

		public static Faker<ClientProperty> ClientPropertyFaker(int id)
		{
			var clientPropertyFaker = new Faker<ClientProperty>()
				.StrictMode(false)
				.RuleFor(o => o.Id, id)
				.RuleFor(o => o.Key, f => Guid.NewGuid().ToString())
				.RuleFor(o => o.Value, f => f.Random.Words(f.Random.Number(1, 5)));

			return clientPropertyFaker;
		}
	}
}
