using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Enums;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class ClientService : IClientService
    {
        protected readonly IClientRepository ClientRepository;
        protected readonly IClientServiceResources ClientServiceResources;
        private const string SharedSecret = "SharedSecret";

        public ClientService(IClientRepository clientRepository, IClientServiceResources clientServiceResources)
        {
            ClientRepository = clientRepository;
            ClientServiceResources = clientServiceResources;
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
                case ClientType.WebHybrid:
                    client.AllowedGrantTypes.AddRange(GrantTypes.Hybrid);
                    break;
                case ClientType.Spa:
                    client.AllowedGrantTypes.AddRange(GrantTypes.Code);                    
                    client.RequirePkce = true;
                    client.RequireClientSecret = false;
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

        public virtual ClientCloneDto BuildClientCloneViewModel(int id, ClientDto clientDto)
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

        public virtual ClientSecretsDto BuildClientSecretsViewModel(ClientSecretsDto clientSecrets)
        {
            clientSecrets.HashTypes = GetHashTypes();
            clientSecrets.TypeList = GetSecretTypes();

            return clientSecrets;
        }

        public virtual ClientDto BuildClientViewModel(ClientDto client = null)
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
        public virtual async Task<int> AddClientAsync(ClientDto client)
        {
            var canInsert = await CanInsertClientAsync(client);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(ClientServiceResources.ClientExistsValue().Description, client.ClientId), ClientServiceResources.ClientExistsKey().Description, client);
            }

            PrepareClientTypeForNewClient(client);
            var clientEntity = client.ToEntity();

            return await ClientRepository.AddClientAsync(clientEntity);
        }

        public virtual async Task<int> UpdateClientAsync(ClientDto client)
        {
            var canInsert = await CanInsertClientAsync(client);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(ClientServiceResources.ClientExistsValue().Description, client.ClientId), ClientServiceResources.ClientExistsKey().Description, client);
            }

            var clientEntity = client.ToEntity();

            return await ClientRepository.UpdateClientAsync(clientEntity);
        }

        public virtual async Task<int> RemoveClientAsync(ClientDto client)
        {
            var clientEntity = client.ToEntity();

            return await ClientRepository.RemoveClientAsync(clientEntity);
        }

        public virtual async Task<int> CloneClientAsync(ClientCloneDto client)
        {
            var canInsert = await CanInsertClientAsync(client, true);
            if (!canInsert)
            {
                //If it failed you need get original clientid, clientname for view title
                var clientInfo = await ClientRepository.GetClientIdAsync(client.Id);
                client.ClientIdOriginal = clientInfo.ClientId;
                client.ClientNameOriginal = clientInfo.ClientName;

                throw new UserFriendlyViewException(string.Format(ClientServiceResources.ClientExistsValue().Description, client.ClientId), ClientServiceResources.ClientExistsKey().Description, client);
            }

            var clientEntity = client.ToEntity();

            var clonedClientId = await ClientRepository.CloneClientAsync(clientEntity, client.CloneClientCorsOrigins,
                client.CloneClientGrantTypes, client.CloneClientIdPRestrictions,
                client.CloneClientPostLogoutRedirectUris,
                client.CloneClientScopes, client.CloneClientRedirectUris, client.CloneClientClaims, client.CloneClientProperties);

            return clonedClientId;
        }

        public virtual Task<bool> CanInsertClientAsync(ClientDto client, bool isCloned = false)
        {
            var clientEntity = client.ToEntity();

            return ClientRepository.CanInsertClientAsync(clientEntity, isCloned);
        }

        public virtual async Task<ClientDto> GetClientAsync(int clientId)
        {
            var client = await ClientRepository.GetClientAsync(clientId);

            if (client == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));

            var clientDto = client.ToModel();

            return clientDto;
        }

        public virtual async Task<ClientsDto> GetClientsAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await ClientRepository.GetClientsAsync(search, page, pageSize);
            var clientsDto = pagedList.ToModel();

            return clientsDto;
        }

        public virtual async Task<List<string>> GetScopesAsync(string scope, int limit = 0)
        {
            var scopes = await ClientRepository.GetScopesAsync(scope, limit);

            return scopes;
        }

        public virtual List<string> GetGrantTypes(string grant, int limit = 0)
        {
            var grantTypes = ClientRepository.GetGrantTypes(grant, limit);

            return grantTypes;
        }

        public virtual List<SelectItem> GetAccessTokenTypes()
        {
            var accessTokenTypes = ClientRepository.GetAccessTokenTypes();

            return accessTokenTypes;
        }

        public virtual List<SelectItem> GetTokenExpirations()
        {
            var tokenExpirations = ClientRepository.GetTokenExpirations();

            return tokenExpirations;
        }

        public virtual List<SelectItem> GetTokenUsage()
        {
            var tokenUsage = ClientRepository.GetTokenUsage();

            return tokenUsage;
        }

        public virtual List<SelectItem> GetHashTypes()
        {
            var hashTypes = ClientRepository.GetHashTypes();

            return hashTypes;
        }

        public virtual List<SelectItem> GetSecretTypes()
        {
            var secretTypes = ClientRepository.GetSecretTypes();

            return secretTypes;
        }

        public virtual List<SelectItem> GetProtocolTypes()
        {
            var protocolTypes = ClientRepository.GetProtocolTypes();

            return protocolTypes;
        }

        public virtual List<string> GetStandardClaims(string claim, int limit = 0)
        {
            var standardClaims = ClientRepository.GetStandardClaims(claim, limit);

            return standardClaims;
        }

        public virtual async Task<int> AddClientSecretAsync(ClientSecretsDto clientSecret)
        {
            HashClientSharedSecret(clientSecret);

            var clientSecretEntity = clientSecret.ToEntity();
            return await ClientRepository.AddClientSecretAsync(clientSecret.ClientId, clientSecretEntity);
        }

        public virtual async Task<int> DeleteClientSecretAsync(ClientSecretsDto clientSecret)
        {
            var clientSecretEntity = clientSecret.ToEntity();

            return await ClientRepository.DeleteClientSecretAsync(clientSecretEntity);
        }

        public virtual async Task<ClientSecretsDto> GetClientSecretsAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var clientInfo = await ClientRepository.GetClientIdAsync(clientId);
            if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));

            var pagedList = await ClientRepository.GetClientSecretsAsync(clientId, page, pageSize);
            var clientSecretsDto = pagedList.ToModel();
            clientSecretsDto.ClientId = clientId;
            clientSecretsDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

            return clientSecretsDto;
        }

        public virtual async Task<ClientSecretsDto> GetClientSecretAsync(int clientSecretId)
        {
            var clientSecret = await ClientRepository.GetClientSecretAsync(clientSecretId);
            if (clientSecret == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientSecretDoesNotExist().Description, clientSecretId));

            var clientInfo = await ClientRepository.GetClientIdAsync(clientSecret.Client.Id);
            if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientSecret.Client.Id));

            var clientSecretsDto = clientSecret.ToModel();
            clientSecretsDto.ClientId = clientSecret.Client.Id;
            clientSecretsDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

            return clientSecretsDto;
        }

        public virtual async Task<ClientClaimsDto> GetClientClaimsAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var clientInfo = await ClientRepository.GetClientIdAsync(clientId);
            if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));

            var pagedList = await ClientRepository.GetClientClaimsAsync(clientId, page, pageSize);
            var clientClaimsDto = pagedList.ToModel();
            clientClaimsDto.ClientId = clientId;
            clientClaimsDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

            return clientClaimsDto;
        }

        public virtual async Task<ClientPropertiesDto> GetClientPropertiesAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var clientInfo = await ClientRepository.GetClientIdAsync(clientId);
            if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientId));

            var pagedList = await ClientRepository.GetClientPropertiesAsync(clientId, page, pageSize);
            var clientPropertiesDto = pagedList.ToModel();
            clientPropertiesDto.ClientId = clientId;
            clientPropertiesDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

            return clientPropertiesDto;
        }

        public virtual async Task<ClientClaimsDto> GetClientClaimAsync(int clientClaimId)
        {
            var clientClaim = await ClientRepository.GetClientClaimAsync(clientClaimId);
            if (clientClaim == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientClaimDoesNotExist().Description, clientClaimId));

            var clientInfo = await ClientRepository.GetClientIdAsync(clientClaim.Client.Id);
            if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientClaim.Client.Id));

            var clientClaimsDto = clientClaim.ToModel();
            clientClaimsDto.ClientId = clientClaim.Client.Id;
            clientClaimsDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

            return clientClaimsDto;
        }

        public virtual async Task<ClientPropertiesDto> GetClientPropertyAsync(int clientPropertyId)
        {
            var clientProperty = await ClientRepository.GetClientPropertyAsync(clientPropertyId);
            if (clientProperty == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientPropertyDoesNotExist().Description, clientPropertyId));

            var clientInfo = await ClientRepository.GetClientIdAsync(clientProperty.Client.Id);
            if (clientInfo.ClientId == null) throw new UserFriendlyErrorPageException(string.Format(ClientServiceResources.ClientDoesNotExist().Description, clientProperty.Client.Id));

            var clientPropertiesDto = clientProperty.ToModel();
            clientPropertiesDto.ClientId = clientProperty.Client.Id;
            clientPropertiesDto.ClientName = ViewHelpers.GetClientName(clientInfo.ClientId, clientInfo.ClientName);

            return clientPropertiesDto;
        }

        public virtual async Task<int> AddClientClaimAsync(ClientClaimsDto clientClaim)
        {
            var clientClaimEntity = clientClaim.ToEntity();

            return await ClientRepository.AddClientClaimAsync(clientClaim.ClientId, clientClaimEntity);
        }

        public virtual async Task<int> AddClientPropertyAsync(ClientPropertiesDto clientProperties)
        {
            var clientProperty = clientProperties.ToEntity();

            return await ClientRepository.AddClientPropertyAsync(clientProperties.ClientId, clientProperty);
        }

        public virtual async Task<int> DeleteClientClaimAsync(ClientClaimsDto clientClaim)
        {
            var clientClaimEntity = clientClaim.ToEntity();

            return await ClientRepository.DeleteClientClaimAsync(clientClaimEntity);
        }

        public virtual async Task<int> DeleteClientPropertyAsync(ClientPropertiesDto clientProperty)
        {
            var clientPropertyEntity = clientProperty.ToEntity();

            return await ClientRepository.DeleteClientPropertyAsync(clientPropertyEntity);
        }
    }
}