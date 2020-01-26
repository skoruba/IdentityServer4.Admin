using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Models;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class ExportService : IExportService
    {
        protected readonly IClientRepository _clientRepository;
        protected readonly IIdentityResourceRepository _identityResourceRepository;
        protected readonly IApiResourceRepository _apiResourceRepository;

        public ExportService(IClientRepository clientRepository, IIdentityResourceRepository identityResourceRepository, IApiResourceRepository apiResourceRepository)
        {
            _clientRepository = clientRepository;
            _identityResourceRepository = identityResourceRepository;
            _apiResourceRepository = apiResourceRepository;
        }

        public async Task<byte[]> GetExportBytesConfigAsync()
        {
            var export = new Export();
            export.Clients = await _clientRepository.GetClientsExportAsync();
            export.IdentityResources = await _identityResourceRepository.GetIdentityResourcesExportAsync();
            export.ApiResources = await _apiResourceRepository.GetApiResourcesExportAsync();
            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(export, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
        }

        public async Task ImportConfigAsync(string txt)
        {
            var config = JsonConvert.DeserializeObject<Export>(txt);
            if (config == null || config.Clients == null)
            {
                throw new UserFriendlyErrorPageException("The file is not a configuration");
            }

            var clients = await _clientRepository.GetClientsExportAsync();
            var identityResources = await _identityResourceRepository.GetIdentityResourcesExportAsync();
            var apiResources = await _apiResourceRepository.GetApiResourcesExportAsync();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var client in clients)
                {
                    await _clientRepository.RemoveClientAsync(client);
                }
                foreach (var identityResource in identityResources)
                {
                    await _identityResourceRepository.DeleteIdentityResourceAsync(identityResource);
                }
                foreach (var apiResource in apiResources)
                {
                    await _apiResourceRepository.DeleteApiResourceAsync(apiResource);
                }

                foreach (var client in config.Clients)
                {
                    await _clientRepository.CloneClientExportAsync(client);
                }
                foreach (var apiResource in config.ApiResources)
                {
                    await _apiResourceRepository.CloneApiResourceAsync(apiResource);
                }
                foreach (var identityResource in config.IdentityResources)
                {
                    await _identityResourceRepository.CloneIdentityResourceAsync(identityResource);
                }

                scope.Complete();
            }
        }
    }
}