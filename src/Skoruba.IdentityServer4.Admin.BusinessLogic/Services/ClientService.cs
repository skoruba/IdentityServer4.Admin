using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Enums;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class ClientService<TDbContext> : IClientService<TDbContext>
        where TDbContext : DbContext, IAdminConfigurationDbContext
    {
        private readonly IClientRepository<TDbContext> _clientRepository;
        private readonly IClientServiceResources _clientServiceResources;
        private const string SharedSecret = "SharedSecret";

        public ClientService(IClientRepository<TDbContext> clientRepository, IClientServiceResources clientServiceResources)
        {
            _clientRepository = clientRepository;
            _clientServiceResources = clientServiceResources;
        }

        private void HashClientSharedSecret(ClientSecretsDto clientSecret)
        {
            if (clientSecret.Type != SharedSecret) return;

            if (clientSecret.HashType == ((int)HashType.Sha256).ToString())
            {
                clientSecret.Value = clientSecret.Value.Sha256();
            }
            else if (clientSecret.HashType == ((int)HashType.Sha512).ToString())
            {
                clientSecret.Value = clientSecret.Value.Sha512();
            }
        }

        private void PrepareClientTypeForNewClient(ClientDto client)
        {
            switch (client.ClientType)
            {
                case ClientType.Empty:
                    break;
                case ClientType.WebImplicit:
                    client.AllowedGrantTypes.AddRange(GrantTypes.Implicit);
                    client.AllowAccessTokensViaBrowser = true;
                    break;
                case ClientType.WebHybrid:
                    client.AllowedGrantTypes.AddRange(GrantTypes.Hybrid);
                    break;
                case ClientType.Spa:
                    client.AllowedGrantTypes.AddRange(GrantTypes.Implicit);
                    client.AllowAccessTokensViaBrowser = true;
                    break;
                case ClientType.Native:
                    client.AllowedGrantTypes.AddRange(GrantTypes.Hybrid);
                    break;
                case ClientType.Machine:
                    client.AllowedGrantTypes.AddRange(GrantTypes.ResourceOwnerPasswordAndClientCredentials);
                    break;
                case ClientType.Device:
                    client.AllowedGrantTypes.AddRange(GrantTypes.DeviceFlow);
                    client.RequireClientSecret = false;
                    client.AllowOfflineAccess = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PopulateClientRelations(ClientDto client)
        {
            ComboBoxHelpers.PopulateValuesToList(client.AllowedScopesItems, client.AllowedScopes);
            ComboBoxHelpers.PopulateValuesToList(client.PostLogoutRedirectUrisItems, client.PostLogoutRedirectUris);
            ComboBoxHelpers.PopulateValuesToList(client.IdentityProviderRestrictionsItems, client.IdentityProviderRestrictions);
            ComboBoxHelpers.PopulateValuesToList(client.RedirectUrisItems, client.RedirectUris);
            ComboBoxHelpers.PopulateValuesToList(client.AllowedCorsOriginsItems, client.AllowedCorsOrigins);
            ComboBoxHelpers.PopulateValuesToList(client.AllowedGrantTypesItems, client.AllowedGrantTypes);
        }

        public ClientCloneDto BuildClientCloneViewModel(int id, ClientDto clientDto)
        {
            var client = new ClientCloneDto
            {
                CloneClientCorsOrigins = true,
                CloneClientGrantTypes = true,
                CloneClientIdPRestrictions = true,
                CloneClientPostLogoutRedirectUris = true,
                CloneClientRedirectUris = true,
                CloneClientScopes = true,
                CloneClientClaims = true,
                CloneClientProperties = true,
                ClientIdOriginal = clientDto.ClientId,
                ClientNameOriginal = clientDto.ClientName,
                Id = id
            };

            return client;
        }

        public ClientSecretsDto BuildClientSecretsViewModel(ClientSecretsDto clientSecrets)
        {
            clientSecrets.HashTypes = GetHashTypes();
            clientSecrets.TypeList = GetSecretTypes();

            return clientSecrets;
        }

        public ClientDto BuildClientViewModel(ClientDto client = null)
        {
            if (client == null)
            {
                var clientDto = new ClientDto
                {
                    AccessTokenTypes = GetAccessTokenTypes(),
                    RefreshTokenExpirations = GetTokenExpirations(),
                    RefreshTokenUsages = GetTokenUsage(),
                    ProtocolTypes = GetProtocolTypes(),
                    Id = 0
                };

                return clientDto;
            }

            client.AccessTokenTypes = GetAccessTokenTypes();
            client.RefreshTokenExpirations = GetTokenExpirations();
            client.RefreshTokenUsages = GetTokenUsage();
            client.ProtocolTypes = GetProtocolTypes();

            PopulateClientRelations(client);

            return client;
        }

        /// <summary>
        /// Add new client, this method doesn't save client secrets, client claims, client properties
        /// </summary>
        /// <param name="client"></param>
        /// <returns>This method return new client id</returns>
        public async Task<int> AddClientAsync(ClientDto client)
        {
            var canInsert = await CanInsertClientAsync(client);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(_clientServiceResources.ClientExistsValue().Description, client.ClientId), _clientServiceResources.ClientExistsKey().Description, client);
            }

            PrepareClientTypeForNewClient(client);
            var clientEntity = client.ToEntity();

            return await _clientRepository.AddClientAsync(clientEntity);
        }

        public async Task<int> UpdateClientAsync(ClientDto client)
        {
            var canInsert = await CanInsertClientAsync(client);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(_clientServiceResources.ClientExistsValue().Description, client.ClientId), _clientServiceResources.ClientExistsKey().Description, client);
            }

            var clientEntity = client.ToEntity();

            return await _clientRepository.UpdateClientAsync(clientEntity);
        }

        public async Task<int> RemoveClientAsync(ClientDto client)
        {
            var clientEntity = client.ToEntity();

            return await _clientRepository.RemoveClientAsync(clientEntity);
        }

        public async Task<int> CloneClientAsync(ClientCloneDto client)
        {
            var canInsert = await CanInsertClientAsync(client, true);
            if (!canInsert)
            {
                //If it failed you need get original clientid, clientname for view title
                var clientInfo = await _clientRepository.GetClientIdAsync(client.Id);
                client.ClientIdOriginal = clientInfo.ClientId;
                client.ClientNameOriginal = clientInfo.ClientName;

                throw new UserFriendlyViewException(string.Format(_clientServiceResources.ClientExistsValue().Description, client.ClientId), _clientServiceResources.ClientExistsKey().Description, client);
            }

            var clientEntity = client.ToEntity();

            var clonedClientId = await _clientRepository.CloneClientAsync(clientEntity, client.CloneClientCorsOrigins,
                client.CloneClientGrantTypes, client.CloneClientIdPRestrictions,
                client.CloneClientPostLogoutRedirectUris,
                client.CloneClientScopes, client.CloneClientRedirectUris, client.CloneClientClaims, client.CloneClientProperties);

            return clonedClientId;
        }

        public Task<bool> CanInsertClientAsync(ClientDto client, bool isCloned = false)
        {
            var clientEntity = client.ToEntity();

            return _clientRepository.CanInsertClientAsync(clientEntity, isCloned);
        }

        public async Task<ClientDto> GetClientAsync(int clientId)
        {
            var client = await _clientRepository.GetClientAsync(clientId);

            if (client == null) throw new UserFriendlyErrorPageException(string.Format(_clientServiceResources.ClientDoesNotExist().Description, clientId));

            var clientDto = client.ToModel();

            return clientDto;
        }

        public async Task<ClientsDto> GetClientsAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await _clientRepository.GetClientsAsync(search, page, pageSize);
            var clientsDto = pagedList.ToModel();

            return clientsDto;
        }

        public async Task<List<string>> GetScopesAsync(string scope, int limit = 0)
        {
            var scopes = await _clientRepository.GetScopesAsync(scope, limit);

            return scopes;
        }

        public List<string> GetGrantTypes(string grant, int limit = 0)
        {
            var grantTypes = _clientRepository.GetGrantTypes(grant, limit);

            return grantTypes;
        }

        public List<SelectItem> GetAccessTokenTypes()
        {
            var accessTokenTypes = _clientRepository.GetAccessTokenTypes();

            return accessTokenTypes;
        }

        public List<SelectItem> GetTokenExpirations()
        {
            var tokenExpirations = _clientRepository.GetTokenExpirations();

            return tokenExpirations;
        }

        public List<SelectItem> GetTokenUsage()
        {
            var tokenUsage = _clientRepository.GetTokenUsage();

            return tokenUsage;
        }

        public List<SelectItem> GetHashTypes()
        {
            var hashTypes = _clientRepository.GetHashTypes();

            return hashTypes;
        }

        public List<SelectItem> GetSecretTypes()
        {
            var secretTypes = _clientRepository.GetSecretTypes();

            return secretTypes;
        }

        public List<SelectItem> GetProtocolTypes()
        {
            var protocolTypes = _clientRepository.GetProtocolTypes();

            return protocolTypes;
        }

        public List<string> GetStandardClaims(string claim, int limit = 0)
        {
            var standardClaims = _clientRepository.GetStandardClaims(claim, limit);

            return standardClaims;
        }

        public async Task<int> AddClientSecretAsync(ClientSecretsDto clientSecret)
        {
            HashClientSharedSecret(clientSecret);

            var clientSecretEntity = clientSecret.ToEntity();
            return await _clientRepository.AddClientSecretAsync(clientSecret.ClientId, clientSecretEntity);
        }

        public async Task<int> DeleteClientSecretAsync(ClientSecretsDto clientSecret)
        {
            var clientSecretEntity = clientSecret.ToEntity();

            return await _clientRepository.DeleteClientSecretAsync(clientSecretEntity);
        }

        public async Task<ClientSecretsDto> GetClientSecretsAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var clientInfo = await _clientRepository.GetClientIdAsync(clientId);
            if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(_clientServiceResources.ClientDoesNotExist().Description, clientId));

            var pagedList = await _clientRepository.GetClientSecretsAsync(clientId, page, pageSize);
            var clientSecretsDto = pagedList.ToModel();
            clientSecretsDto.ClientId = clientId;
            clientSecretsDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

            return clientSecretsDto;
        }

        public async Task<ClientSecretsDto> GetClientSecretAsync(int clientSecretId)
        {
            var clientSecret = await _clientRepository.GetClientSecretAsync(clientSecretId);
            if (clientSecret == null) throw new UserFriendlyErrorPageException(string.Format(_clientServiceResources.ClientSecretDoesNotExist().Description, clientSecretId));

            var clientInfo = await _clientRepository.GetClientIdAsync(clientSecret.Client.Id);
            if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(_clientServiceResources.ClientDoesNotExist().Description, clientSecret.Client.Id));

            var clientSecretsDto = clientSecret.ToModel();
            clientSecretsDto.ClientId = clientSecret.Client.Id;
            clientSecretsDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

            return clientSecretsDto;
        }

        public async Task<ClientClaimsDto> GetClientClaimsAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var clientInfo = await _clientRepository.GetClientIdAsync(clientId);
            if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(_clientServiceResources.ClientDoesNotExist().Description, clientId));

            var pagedList = await _clientRepository.GetClientClaimsAsync(clientId, page, pageSize);
            var clientClaimsDto = pagedList.ToModel();
            clientClaimsDto.ClientId = clientId;
            clientClaimsDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

            return clientClaimsDto;
        }

        public async Task<ClientPropertiesDto> GetClientPropertiesAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var clientInfo = await _clientRepository.GetClientIdAsync(clientId);
            if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(_clientServiceResources.ClientDoesNotExist().Description, clientId));

            var pagedList = await _clientRepository.GetClientPropertiesAsync(clientId, page, pageSize);
            var clientPropertiesDto = pagedList.ToModel();
            clientPropertiesDto.ClientId = clientId;
            clientPropertiesDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

            return clientPropertiesDto;
        }

        public async Task<ClientClaimsDto> GetClientClaimAsync(int clientClaimId)
        {
            var clientClaim = await _clientRepository.GetClientClaimAsync(clientClaimId);
            if (clientClaim == null) throw new UserFriendlyErrorPageException(string.Format(_clientServiceResources.ClientClaimDoesNotExist().Description, clientClaimId));

            var clientInfo = await _clientRepository.GetClientIdAsync(clientClaim.Client.Id);
            if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(_clientServiceResources.ClientDoesNotExist().Description, clientClaim.Client.Id));

            var clientClaimsDto = clientClaim.ToModel();
            clientClaimsDto.ClientId = clientClaim.Client.Id;
            clientClaimsDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

            return clientClaimsDto;
        }

        public async Task<ClientPropertiesDto> GetClientPropertyAsync(int clientPropertyId)
        {
            var clientProperty = await _clientRepository.GetClientPropertyAsync(clientPropertyId);
            if (clientProperty == null) throw new UserFriendlyErrorPageException(string.Format(_clientServiceResources.ClientPropertyDoesNotExist().Description, clientPropertyId));

            var clientInfo = await _clientRepository.GetClientIdAsync(clientProperty.Client.Id);
            if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(_clientServiceResources.ClientDoesNotExist().Description, clientProperty.Client.Id));

            var clientPropertiesDto = clientProperty.ToModel();
            clientPropertiesDto.ClientId = clientProperty.Client.Id;
            clientPropertiesDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

            return clientPropertiesDto;
        }

        public async Task<int> AddClientClaimAsync(ClientClaimsDto clientClaim)
        {
            var clientClaimEntity = clientClaim.ToEntity();

            return await _clientRepository.AddClientClaimAsync(clientClaim.ClientId, clientClaimEntity);
        }

        public async Task<int> AddClientPropertyAsync(ClientPropertiesDto clientProperties)
        {
            var clientProperty = clientProperties.ToEntity();

            return await _clientRepository.AddClientPropertyAsync(clientProperties.ClientId, clientProperty);
        }

        public async Task<int> DeleteClientClaimAsync(ClientClaimsDto clientClaim)
        {
            var clientClaimEntity = clientClaim.ToEntity();

            return await _clientRepository.DeleteClientClaimAsync(clientClaimEntity);
        }

        public async Task<int> DeleteClientPropertyAsync(ClientPropertiesDto clientProperty)
        {
            var clientPropertyEntity = clientProperty.ToEntity();

            return await _clientRepository.DeleteClientPropertyAsync(clientPropertyEntity);
        }
    }
}