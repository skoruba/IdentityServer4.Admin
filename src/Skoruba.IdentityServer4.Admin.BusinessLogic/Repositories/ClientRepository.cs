using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Constants;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Enums;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories
{
    public class ClientRepository<TDbContext> : IClientRepository<TDbContext>
    where TDbContext : DbContext, IAdminConfigurationDbContext
    {
        private readonly TDbContext _dbContext;
        public bool AutoSaveChanges { get; set; } = true;

        public ClientRepository(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Client> GetClientAsync(int clientId)
        {
            return _dbContext.Clients
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
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<Client>> GetClientsAsync(string search = "", int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<Client>();

            Expression<Func<Client, bool>> searchCondition = x => x.ClientId.Contains(search) || x.ClientName.Contains(search);
            var clients = await _dbContext.Clients.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.Id, page, pageSize).ToListAsync();
            pagedList.Data.AddRange(clients);
            pagedList.TotalCount = await _dbContext.Clients.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public async Task<List<string>> GetScopesAsync(string scope, int limit = 0)
        {
            var identityResources = await _dbContext.IdentityResources
                .WhereIf(!string.IsNullOrEmpty(scope), x => x.Name.Contains(scope))
                .TakeIf(x => x.Id, limit > 0, limit)
                .Select(x => x.Name).ToListAsync();

            var apiScopes = await _dbContext.ApiScopes
                .WhereIf(!string.IsNullOrEmpty(scope), x => x.Name.Contains(scope))
                .TakeIf(x => x.Id, limit > 0, limit)
                .Select(x => x.Name).ToListAsync();

            var scopes = identityResources.Concat(apiScopes).TakeIf(x => x, limit > 0, limit).ToList();

            return scopes;
        }

        public List<string> GetGrantTypes(string grant, int limit = 0)
        {
            var filteredGrants = ClientConsts.GetGrantTypes()
                .WhereIf(!string.IsNullOrWhiteSpace(grant), x => x.Contains(grant))
                .TakeIf(x => x, limit > 0, limit)
                .ToList();

            return filteredGrants;
        }

        public List<SelectItem> GetProtocolTypes()
        {
            return ClientConsts.GetProtocolTypes();
        }

        public List<SelectItem> GetSecretTypes()
        {
            var secrets = new List<SelectItem>();
            secrets.AddRange(ClientConsts.GetSecretTypes().Select(x => new SelectItem(x, x)));

            return secrets;
        }

        public List<string> GetStandardClaims(string claim, int limit = 0)
        {
            var filteredClaims = ClientConsts.GetStandardClaims()
                .WhereIf(!string.IsNullOrWhiteSpace(claim), x => x.Contains(claim))
                .TakeIf(x => x, limit > 0, limit)
                .ToList();

            return filteredClaims;
        }

        public List<SelectItem> GetAccessTokenTypes()
        {
            var accessTokenTypes = EnumHelpers.ToSelectList<AccessTokenType>();
            return accessTokenTypes;
        }

        public List<SelectItem> GetTokenExpirations()
        {
            var tokenExpirations = EnumHelpers.ToSelectList<TokenExpiration>();
            return tokenExpirations;
        }

        public List<SelectItem> GetTokenUsage()
        {
            var tokenUsage = EnumHelpers.ToSelectList<TokenUsage>();
            return tokenUsage;
        }

        public List<SelectItem> GetHashTypes()
        {
            var hashTypes = EnumHelpers.ToSelectList<HashType>();
            return hashTypes;
        }

        public async Task<int> AddClientSecretAsync(int clientId, ClientSecret clientSecret)
        {
            var client = await _dbContext.Clients.Where(x => x.Id == clientId).SingleOrDefaultAsync();
            clientSecret.Client = client;

            await _dbContext.ClientSecrets.AddAsync(clientSecret);

            return await AutoSaveChangesAsync();
        }

        private async Task<int> AutoSaveChangesAsync()
        {
            return AutoSaveChanges ? await _dbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
        }

        public Task<ClientProperty> GetClientPropertyAsync(int clientPropertyId)
        {
            return _dbContext.ClientProperties
                .Include(x => x.Client)
                .Where(x => x.Id == clientPropertyId)
                .SingleOrDefaultAsync();
        }

        public async Task<int> AddClientClaimAsync(int clientId, ClientClaim clientClaim)
        {
            var client = await _dbContext.Clients.Where(x => x.Id == clientId).SingleOrDefaultAsync();

            clientClaim.Client = client;
            await _dbContext.ClientClaims.AddAsync(clientClaim);

            return await AutoSaveChangesAsync();
        }

        public async Task<int> AddClientPropertyAsync(int clientId, ClientProperty clientProperty)
        {
            var client = await _dbContext.Clients.Where(x => x.Id == clientId).SingleOrDefaultAsync();

            clientProperty.Client = client;
            await _dbContext.ClientProperties.AddAsync(clientProperty);

            return await AutoSaveChangesAsync();
        }

        public async Task<(string ClientId, string ClientName)> GetClientIdAsync(int clientId)
        {
            var client = await _dbContext.Clients.Where(x => x.Id == clientId)
                .Select(x => new { x.ClientId, x.ClientName })
                .SingleOrDefaultAsync();

            return (client?.ClientId, client?.ClientName);
        }

        public async Task<PagedList<ClientSecret>> GetClientSecretsAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ClientSecret>();

            var secrets = await _dbContext.ClientSecrets
                .Where(x => x.Client.Id == clientId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(secrets);
            pagedList.TotalCount = await _dbContext.ClientSecrets.Where(x => x.Client.Id == clientId).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public Task<ClientSecret> GetClientSecretAsync(int clientSecretId)
        {
            return _dbContext.ClientSecrets
                .Include(x => x.Client)
                .Where(x => x.Id == clientSecretId)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<ClientClaim>> GetClientClaimsAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ClientClaim>();

            var claims = await _dbContext.ClientClaims.Where(x => x.Client.Id == clientId).PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(claims);
            pagedList.TotalCount = await _dbContext.ClientClaims.Where(x => x.Client.Id == clientId).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public async Task<PagedList<ClientProperty>> GetClientPropertiesAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<ClientProperty>();

            var properties = await _dbContext.ClientProperties.Where(x => x.Client.Id == clientId).PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(properties);
            pagedList.TotalCount = await _dbContext.ClientProperties.Where(x => x.Client.Id == clientId).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public Task<ClientClaim> GetClientClaimAsync(int clientClaimId)
        {
            return _dbContext.ClientClaims
                .Include(x => x.Client)
                .Where(x => x.Id == clientClaimId)
                .SingleOrDefaultAsync();
        }

        public async Task<int> DeleteClientSecretAsync(ClientSecret clientSecret)
        {
            var secretToDelete = await _dbContext.ClientSecrets.Where(x => x.Id == clientSecret.Id).SingleOrDefaultAsync();

            _dbContext.ClientSecrets.Remove(secretToDelete);

            return await AutoSaveChangesAsync();
        }

        public async Task<int> DeleteClientClaimAsync(ClientClaim clientClaim)
        {
            var claimToDelete = await _dbContext.ClientClaims.Where(x => x.Id == clientClaim.Id).SingleOrDefaultAsync();

            _dbContext.ClientClaims.Remove(claimToDelete);
            return await AutoSaveChangesAsync();
        }

        public async Task<int> DeleteClientPropertyAsync(ClientProperty clientProperty)
        {
            var propertyToDelete = await _dbContext.ClientProperties.Where(x => x.Id == clientProperty.Id).SingleOrDefaultAsync();

            _dbContext.ClientProperties.Remove(propertyToDelete);
            return await AutoSaveChangesAsync();
        }

        public async Task<int> SaveAllChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> CanInsertClientAsync(Client client, bool isCloned = false)
        {
            if (client.Id == 0 || isCloned)
            {
                var existsWithClientName = await _dbContext.Clients.Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();
                return existsWithClientName == null;
            }
            else
            {
                var existsWithClientName = await _dbContext.Clients.Where(x => x.ClientId == client.ClientId && x.Id != client.Id).SingleOrDefaultAsync();
                return existsWithClientName == null;
            }
        }

        /// <summary>
        /// Add new client, this method doesn't save client secrets, client claims, client properties
        /// </summary>
        /// <param name="client"></param>
        /// <returns>This method return new client id</returns>
        public async Task<int> AddClientAsync(Client client)
        {
            _dbContext.Clients.Add(client);

            await AutoSaveChangesAsync();

            return client.Id;
        }

        public async Task<int> CloneClientAsync(Client client,
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
            var clientToClone = await _dbContext.Clients
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

            await _dbContext.Clients.AddAsync(clientToClone);

            await AutoSaveChangesAsync();

            var id = clientToClone.Id;

            return id;
        }

        private async Task RemoveClientRelationsAsync(Client client)
        {
            //Remove old allowed scopes
            var clientScopes = await _dbContext.ClientScopes.Where(x => x.Client.Id == client.Id).ToListAsync();
            _dbContext.ClientScopes.RemoveRange(clientScopes);

            //Remove old grant types
            var clientGrantTypes = await _dbContext.ClientGrantTypes.Where(x => x.Client.Id == client.Id).ToListAsync();
            _dbContext.ClientGrantTypes.RemoveRange(clientGrantTypes);

            //Remove old redirect uri
            var clientRedirectUris = await _dbContext.ClientRedirectUris.Where(x => x.Client.Id == client.Id).ToListAsync();
            _dbContext.ClientRedirectUris.RemoveRange(clientRedirectUris);

            //Remove old client cors
            var clientCorsOrigins = await _dbContext.ClientCorsOrigins.Where(x => x.Client.Id == client.Id).ToListAsync();
            _dbContext.ClientCorsOrigins.RemoveRange(clientCorsOrigins);

            //Remove old client id restrictions
            var clientIdPRestrictions = await _dbContext.ClientIdPRestrictions.Where(x => x.Client.Id == client.Id).ToListAsync();
            _dbContext.ClientIdPRestrictions.RemoveRange(clientIdPRestrictions);

            //Remove old client post logout redirect
            var clientPostLogoutRedirectUris = await _dbContext.ClientPostLogoutRedirectUris.Where(x => x.Client.Id == client.Id).ToListAsync();
            _dbContext.ClientPostLogoutRedirectUris.RemoveRange(clientPostLogoutRedirectUris);
        }

        public async Task<int> UpdateClientAsync(Client client)
        {
            //Remove old relations
            await RemoveClientRelationsAsync(client);

            //Update with new data
            _dbContext.Clients.Update(client);

            return await AutoSaveChangesAsync();
        }

        public async Task<int> RemoveClientAsync(Client client)
        {
            _dbContext.Clients.Remove(client);

            return await AutoSaveChangesAsync();
        }
    }
}