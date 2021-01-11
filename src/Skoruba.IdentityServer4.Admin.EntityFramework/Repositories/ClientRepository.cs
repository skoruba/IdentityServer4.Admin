using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Enums;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Extensions;
using Skoruba.IdentityServer4.Admin.EntityFramework.Helpers;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Repositories
{
    public class ClientRepository<TDbContext> : IClientRepository
    where TDbContext : DbContext, IAdminConfigurationDbContext
    {
        protected readonly TDbContext DbContext;
        public bool AutoSaveChanges { get; set; } = true;

        public ClientRepository(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual Task<Client> GetClientAsync(int clientId)
        {
            return DbContext.Clients
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.AllowedScopes)
                .Include(x => x.ClientSecrets)
                .Include(x => x.Claims)
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.Properties)
                .Where(x => x.Id == clientId)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public virtual async Task<PagedList<Client>> GetClientsAsync(string search = "", int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<Client>();

            Expression<Func<Client, bool>> searchCondition = x => x.ClientId.Contains(search) || x.ClientName.Contains(search);
            var clients = await DbContext.Clients.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.Id, page, pageSize).ToListAsync();
            pagedList.Data.AddRange(clients);
            pagedList.TotalCount = await DbContext.Clients.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual async Task<List<string>> GetScopesAsync(string scope, int limit = 0)
        {
            var identityResources = await DbContext.IdentityResources
                .WhereIf(!string.IsNullOrEmpty(scope), x => x.Name.Contains(scope))
                .TakeIf(x => x.Id, limit > 0, limit)
                .Select(x => x.Name).ToListAsync();

            var apiScopes = await DbContext.ApiScopes
                .WhereIf(!string.IsNullOrEmpty(scope), x => x.Name.Contains(scope))
                .TakeIf(x => x.Id, limit > 0, limit)
                .Select(x => x.Name).ToListAsync();

            var scopes = identityResources.Concat(apiScopes).TakeIf(x => x, limit > 0, limit).ToList();

            return scopes;
        }

        public virtual List<string> GetGrantTypes(string grant, int limit = 0)
        {
            var filteredGrants = ClientConsts.GetGrantTypes()
                .WhereIf(!string.IsNullOrWhiteSpace(grant), x => x.Contains(grant))
                .TakeIf(x => x, limit > 0, limit)
                .ToList();

            return filteredGrants;
        }

        public virtual List<SelectItem> GetProtocolTypes()
        {
            return ClientConsts.GetProtocolTypes();
        }

        public virtual List<SelectItem> GetSecretTypes()
        {
            var secrets = new List<SelectItem>();
            secrets.AddRange(ClientConsts.GetSecretTypes().Select(x => new SelectItem(x, x)));

            return secrets;
        }

        public virtual List<string> GetStandardClaims(string claim, int limit = 0)
        {
            var filteredClaims = ClientConsts.GetStandardClaims()
                .WhereIf(!string.IsNullOrWhiteSpace(claim), x => x.Contains(claim))
                .TakeIf(x => x, limit > 0, limit)
                .ToList();

            return filteredClaims;
        }

        public virtual List<SelectItem> GetAccessTokenTypes()
        {
            var accessTokenTypes = EnumHelpers.ToSelectList<AccessTokenType>();
            return accessTokenTypes;
        }

        public virtual List<SelectItem> GetTokenExpirations()
        {
            var tokenExpirations = EnumHelpers.ToSelectList<TokenExpiration>();
            return tokenExpirations;
        }

        public virtual List<SelectItem> GetTokenUsage()
        {
            var tokenUsage = EnumHelpers.ToSelectList<TokenUsage>();
            return tokenUsage;
        }

        public virtual List<SelectItem> GetHashTypes()
        {
            var hashTypes = EnumHelpers.ToSelectList<HashType>();
            return hashTypes;
        }

        public virtual async Task<int> AddClientSecretAsync(int clientId, ClientSecret clientSecret)
        {
            var client = await DbContext.Clients.Where(x => x.Id == clientId).SingleOrDefaultAsync();
            clientSecret.Client = client;

            await DbContext.ClientSecrets.AddAsync(clientSecret);

            return await AutoSaveChangesAsync();
        }

        protected virtual async Task<int> AutoSaveChangesAsync()
        {
            return AutoSaveChanges ? await DbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
        }

        public virtual Task<ClientProperty> GetClientPropertyAsync(int clientPropertyId)
        {
            return DbContext.ClientProperties
                .Include(x => x.Client)
                .Where(x => x.Id == clientPropertyId)
                .SingleOrDefaultAsync();
        }

        public virtual async Task<int> AddClientClaimAsync(int clientId, ClientClaim clientClaim)
        {
            var client = await DbContext.Clients.Where(x => x.Id == clientId).SingleOrDefaultAsync();

            clientClaim.Client = client;
            await DbContext.ClientClaims.AddAsync(clientClaim);

            return await AutoSaveChangesAsync();
        }

        public virtual async Task<int> AddClientPropertyAsync(int clientId, ClientProperty clientProperty)
        {
            var client = await DbContext.Clients.Where(x => x.Id == clientId).SingleOrDefaultAsync();

            clientProperty.Client = client;
            await DbContext.ClientProperties.AddAsync(clientProperty);

            return await AutoSaveChangesAsync();
        }

        public virtual async Task<(string ClientId, string ClientName)> GetClientIdAsync(int clientId)
        {
            var client = await DbContext.Clients.Where(x => x.Id == clientId)
                .Select(x => new { x.ClientId, x.ClientName })
                .SingleOrDefaultAsync();

            return (client?.ClientId, client?.ClientName);
        }

        public virtual async Task<PagedList<ClientSecret>> GetClientSecretsAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ClientSecret>();

            var secrets = await DbContext.ClientSecrets
                .Where(x => x.Client.Id == clientId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(secrets);
            pagedList.TotalCount = await DbContext.ClientSecrets.Where(x => x.Client.Id == clientId).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual Task<ClientSecret> GetClientSecretAsync(int clientSecretId)
        {
            return DbContext.ClientSecrets
                .Include(x => x.Client)
                .Where(x => x.Id == clientSecretId)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public virtual async Task<PagedList<ClientClaim>> GetClientClaimsAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ClientClaim>();

            var claims = await DbContext.ClientClaims.Where(x => x.Client.Id == clientId).PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(claims);
            pagedList.TotalCount = await DbContext.ClientClaims.Where(x => x.Client.Id == clientId).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual async Task<PagedList<ClientProperty>> GetClientPropertiesAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ClientProperty>();

            var properties = await DbContext.ClientProperties.Where(x => x.Client.Id == clientId).PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(properties);
            pagedList.TotalCount = await DbContext.ClientProperties.Where(x => x.Client.Id == clientId).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual Task<ClientClaim> GetClientClaimAsync(int clientClaimId)
        {
            return DbContext.ClientClaims
                .Include(x => x.Client)
                .Where(x => x.Id == clientClaimId)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public virtual async Task<int> DeleteClientSecretAsync(ClientSecret clientSecret)
        {
            var secretToDelete = await DbContext.ClientSecrets.Where(x => x.Id == clientSecret.Id).SingleOrDefaultAsync();

            DbContext.ClientSecrets.Remove(secretToDelete);

            return await AutoSaveChangesAsync();
        }

        public virtual async Task<int> DeleteClientClaimAsync(ClientClaim clientClaim)
        {
            var claimToDelete = await DbContext.ClientClaims.Where(x => x.Id == clientClaim.Id).SingleOrDefaultAsync();

            DbContext.ClientClaims.Remove(claimToDelete);
            return await AutoSaveChangesAsync();
        }

        public virtual async Task<int> DeleteClientPropertyAsync(ClientProperty clientProperty)
        {
            var propertyToDelete = await DbContext.ClientProperties.Where(x => x.Id == clientProperty.Id).SingleOrDefaultAsync();

            DbContext.ClientProperties.Remove(propertyToDelete);
            return await AutoSaveChangesAsync();
        }

        public virtual async Task<int> SaveAllChangesAsync()
        {
            return await DbContext.SaveChangesAsync();
        }

        public virtual async Task<bool> CanInsertClientAsync(Client client, bool isCloned = false)
        {
            if (client.Id == 0 || isCloned)
            {
                var existsWithClientName = await DbContext.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();
                return existsWithClientName == null;
            }
            else
            {
                var existsWithClientName = await DbContext.Clients.Where(x => x.ClientId == client.ClientId && x.Id != client.Id).SingleOrDefaultAsync();
                return existsWithClientName == null;
            }
        }

        /// <summary>
        /// Add new client, this method doesn't save client secrets, client claims, client properties
        /// </summary>
        /// <param name="client"></param>
        /// <returns>This method return new client id</returns>
        public virtual async Task<int> AddClientAsync(Client client)
        {
            DbContext.Clients.Add(client);

            await AutoSaveChangesAsync();

            return client.Id;
        }

        public virtual async Task<int> CloneClientAsync(Client client,
            bool cloneClientCorsOrigins = true,
            bool cloneClientGrantTypes = true,
            bool cloneClientIdPRestrictions = true,
            bool cloneClientPostLogoutRedirectUris = true,
            bool cloneClientScopes = true,
            bool cloneClientRedirectUris = true,
            bool cloneClientClaims = true,
            bool cloneClientProperties = true
            )
        {
            var clientToClone = await DbContext.Clients
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.AllowedScopes)
                .Include(x => x.ClientSecrets)
                .Include(x => x.Claims)
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.Properties)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == client.Id);

            clientToClone.ClientName = client.ClientName;
            clientToClone.ClientId = client.ClientId;

            //Clean original ids
            clientToClone.Id = 0;
            clientToClone.AllowedCorsOrigins.ForEach(x => x.Id = 0);
            clientToClone.RedirectUris.ForEach(x => x.Id = 0);
            clientToClone.PostLogoutRedirectUris.ForEach(x => x.Id = 0);
            clientToClone.AllowedScopes.ForEach(x => x.Id = 0);
            clientToClone.ClientSecrets.ForEach(x => x.Id = 0);
            clientToClone.IdentityProviderRestrictions.ForEach(x => x.Id = 0);
            clientToClone.Claims.ForEach(x => x.Id = 0);
            clientToClone.AllowedGrantTypes.ForEach(x => x.Id = 0);
            clientToClone.Properties.ForEach(x => x.Id = 0);

            //Client secret will be skipped
            clientToClone.ClientSecrets.Clear();

            if (!cloneClientCorsOrigins)
            {
                clientToClone.AllowedCorsOrigins.Clear();
            }

            if (!cloneClientGrantTypes)
            {
                clientToClone.AllowedGrantTypes.Clear();
            }

            if (!cloneClientIdPRestrictions)
            {
                clientToClone.IdentityProviderRestrictions.Clear();
            }

            if (!cloneClientPostLogoutRedirectUris)
            {
                clientToClone.PostLogoutRedirectUris.Clear();
            }

            if (!cloneClientScopes)
            {
                clientToClone.AllowedScopes.Clear();
            }

            if (!cloneClientRedirectUris)
            {
                clientToClone.RedirectUris.Clear();
            }

            if (!cloneClientClaims)
            {
                clientToClone.Claims.Clear();
            }

            if (!cloneClientProperties)
            {
                clientToClone.Properties.Clear();
            }

            await DbContext.Clients.AddAsync(clientToClone);

            await AutoSaveChangesAsync();

            var id = clientToClone.Id;

            return id;
        }

        public virtual async Task<int> UpdateClientAsync(Client client)
        {
            var clientToUpdate = await GetClientAsync(client.Id);

            // Scopes
            var scopesToRemove = clientToUpdate.AllowedScopes.Where(o => !client.AllowedScopes.Any(n => o.Scope == n.Scope));
            DbContext.ClientScopes.RemoveRange(scopesToRemove);

            var newScopes = client.AllowedScopes.Where(n => !clientToUpdate.AllowedScopes.Any(o => o.Scope == n.Scope));
            clientToUpdate.AllowedScopes.AddRange(newScopes);

            //Grant types
            var grantTypesToRemove = clientToUpdate.AllowedGrantTypes.Where(o => !client.AllowedGrantTypes.Any(n => o.GrantType == n.GrantType));
            DbContext.ClientGrantTypes.RemoveRange(grantTypesToRemove);

            var newGrantTypes = client.AllowedGrantTypes.Where(n => !clientToUpdate.AllowedGrantTypes.Any(o => o.GrantType == n.GrantType));
            clientToUpdate.AllowedGrantTypes.AddRange(newGrantTypes);

            // Redirect URIs
            var redirectUrisToRemove = clientToUpdate.RedirectUris.Where(o => !client.RedirectUris.Any(n => o.RedirectUri == n.RedirectUri));
            DbContext.ClientRedirectUris.RemoveRange(redirectUrisToRemove);

            var newRedirectUris = client.RedirectUris.Where(n => !clientToUpdate.RedirectUris.Any(o => o.RedirectUri == n.RedirectUri));
            clientToUpdate.RedirectUris.AddRange(newRedirectUris);

            // Post logout redirect URIs
            var postLogoutRedirectUrisToRemove = clientToUpdate.PostLogoutRedirectUris.Where(o => !client.PostLogoutRedirectUris.Any(n => o.PostLogoutRedirectUri == n.PostLogoutRedirectUri));
            DbContext.ClientPostLogoutRedirectUris.RemoveRange(postLogoutRedirectUrisToRemove);

            var newPostLogoutRedirectUris = client.PostLogoutRedirectUris.Where(n => !clientToUpdate.PostLogoutRedirectUris.Any(o => o.PostLogoutRedirectUri == n.PostLogoutRedirectUri));
            clientToUpdate.PostLogoutRedirectUris.AddRange(newPostLogoutRedirectUris);

            // Client CORS
            var clientCorsOriginsToRemove = clientToUpdate.AllowedCorsOrigins.Where(o => !client.AllowedCorsOrigins.Any(n => o.Origin == n.Origin));
            DbContext.ClientCorsOrigins.RemoveRange(clientCorsOriginsToRemove);

            var newClientCorsOrigins = client.AllowedCorsOrigins.Where(n => !clientToUpdate.AllowedCorsOrigins.Any(o => o.Origin == n.Origin));
            clientToUpdate.AllowedCorsOrigins.AddRange(newClientCorsOrigins);

            // Client id restrictions
            var clientIdPRestrictionsToRemove = clientToUpdate.IdentityProviderRestrictions.Where(o => !client.IdentityProviderRestrictions.Any(n => o.Provider == n.Provider));
            DbContext.ClientIdPRestrictions.RemoveRange(clientIdPRestrictionsToRemove);

            var newClientIdPRestrictions = client.IdentityProviderRestrictions.Where(n => !clientToUpdate.IdentityProviderRestrictions.Any(o => o.Provider == n.Provider));
            clientToUpdate.IdentityProviderRestrictions.AddRange(newClientIdPRestrictions);

            //Update with new data
            clientToUpdate.ClientId = client.ClientId;
            clientToUpdate.Updated = DateTime.Now;
            clientToUpdate.AuthorizationCodeLifetime = client.AuthorizationCodeLifetime;
            clientToUpdate.ConsentLifetime = client.ConsentLifetime;
            clientToUpdate.AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime;
            clientToUpdate.SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime;
            clientToUpdate.RefreshTokenUsage = client.RefreshTokenUsage;
            clientToUpdate.UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh;
            clientToUpdate.RefreshTokenExpiration = client.RefreshTokenExpiration;
            clientToUpdate.AccessTokenType = client.AccessTokenType;
            clientToUpdate.EnableLocalLogin = client.EnableLocalLogin;
            clientToUpdate.AccessTokenLifetime = client.AccessTokenLifetime;
            clientToUpdate.IncludeJwtId = client.IncludeJwtId;
            clientToUpdate.AlwaysSendClientClaims = client.AlwaysSendClientClaims;
            clientToUpdate.ClientClaimsPrefix = client.ClientClaimsPrefix;
            clientToUpdate.PairWiseSubjectSalt = client.PairWiseSubjectSalt;
            clientToUpdate.UserSsoLifetime = client.UserSsoLifetime;
            clientToUpdate.UserCodeType = client.UserCodeType;
            clientToUpdate.IdentityTokenLifetime = client.IdentityTokenLifetime;
            clientToUpdate.AllowOfflineAccess = client.AllowOfflineAccess;
            clientToUpdate.Enabled = client.Enabled;
            clientToUpdate.ProtocolType = client.ProtocolType;
            clientToUpdate.RequireClientSecret = client.RequireClientSecret;
            clientToUpdate.ClientName = client.ClientName;
            clientToUpdate.Description = client.Description;
            clientToUpdate.ClientUri = client.ClientUri;
            clientToUpdate.LogoUri = client.LogoUri;
            clientToUpdate.RequireConsent = client.RequireConsent;
            clientToUpdate.AllowRememberConsent = client.AllowRememberConsent;
            clientToUpdate.AlwaysIncludeUserClaimsInIdToken = client.AlwaysIncludeUserClaimsInIdToken;
            clientToUpdate.RequirePkce = client.RequirePkce;
            clientToUpdate.AllowPlainTextPkce = client.AllowPlainTextPkce;
            clientToUpdate.AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser;
            clientToUpdate.FrontChannelLogoutUri = client.FrontChannelLogoutUri;
            clientToUpdate.FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired;
            clientToUpdate.BackChannelLogoutUri = client.BackChannelLogoutUri;
            clientToUpdate.BackChannelLogoutSessionRequired = client.BackChannelLogoutSessionRequired;
            clientToUpdate.DeviceCodeLifetime = client.DeviceCodeLifetime;
            clientToUpdate.NonEditable = client.NonEditable;

            DbContext.Clients.Update(clientToUpdate);

            return await AutoSaveChangesAsync();
        }

        public virtual async Task<int> RemoveClientAsync(Client client)
        {
            DbContext.Clients.Remove(client);

            return await AutoSaveChangesAsync();
        }
    }
}