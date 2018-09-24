using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
    public interface IClientService<TDbContext> where TDbContext : DbContext, IAdminConfigurationDbContext
    {
        ClientDto BuildClientViewModel(ClientDto client = null);

        ClientSecretsDto BuildClientSecretsViewModel(ClientSecretsDto clientSecrets);

        ClientCloneDto BuildClientCloneViewModel(int id, ClientDto clientDto);

        Task<int> AddClientAsync(ClientDto client);

        Task<int> UpdateClientAsync(ClientDto client);

        Task<int> RemoveClientAsync(ClientDto client);

        Task<int> CloneClientAsync(ClientCloneDto client);

        Task<bool> CanInsertClientAsync(ClientDto client, bool isCloned = false);

        Task<ClientDto> GetClientAsync(int clientId);

        Task<ClientsDto> GetClientsAsync(string search, int page = 1, int pageSize = 10);

        Task<List<string>> GetScopesAsync(string scope, int limit = 0);

        List<string> GetGrantTypes(string grant, int limit = 0);

        List<SelectItem> GetAccessTokenTypes();

        List<SelectItem> GetTokenExpirations();

        List<SelectItem> GetTokenUsage();

        List<SelectItem> GetHashTypes();

        List<SelectItem> GetSecretTypes();

        List<string> GetStandardClaims(string claim, int limit = 0);

        Task<int> AddClientSecretAsync(ClientSecretsDto clientSecret);

        Task<int> DeleteClientSecretAsync(ClientSecretsDto clientSecret);

        Task<ClientSecretsDto> GetClientSecretsAsync(int clientId, int page = 1, int pageSize = 10);

        Task<ClientSecretsDto> GetClientSecretAsync(int clientSecretId);

        Task<ClientClaimsDto> GetClientClaimsAsync(int clientId, int page = 1, int pageSize = 10);

        Task<ClientPropertiesDto> GetClientPropertiesAsync(int clientId, int page = 1, int pageSize = 10);

        Task<ClientClaimsDto> GetClientClaimAsync(int clientClaimId);

        Task<ClientPropertiesDto> GetClientPropertyAsync(int clientPropertyId);

        Task<int> AddClientClaimAsync(ClientClaimsDto clientClaim);

        Task<int> AddClientPropertyAsync(ClientPropertiesDto clientProperties);

        Task<int> DeleteClientClaimAsync(ClientClaimsDto clientClaim);

        Task<int> DeleteClientPropertyAsync(ClientPropertiesDto clientProperty);

        List<SelectItem> GetProtocolTypes();
    }
}