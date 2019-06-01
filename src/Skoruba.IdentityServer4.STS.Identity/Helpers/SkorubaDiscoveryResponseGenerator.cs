using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    class SkorubaDiscoveryResponseGenerator : DiscoveryResponseGenerator
    {
        public SkorubaDiscoveryResponseGenerator(IdentityServerOptions options, IResourceStore resourceStore, IKeyMaterialService keys, ExtensionGrantValidator extensionGrants, SecretParser secretParsers, IResourceOwnerPasswordValidator resourceOwnerValidator, ILogger<DiscoveryResponseGenerator> logger) : base(options, resourceStore, keys, extensionGrants, secretParsers, resourceOwnerValidator, logger)
        {
        }

        public override async Task<Dictionary<string, object>> CreateDiscoveryDocumentAsync(string baseUrl, string issuerUri) {
            var result = await base.CreateDiscoveryDocumentAsync(baseUrl, issuerUri);

            result[OidcConstants.Discovery.AuthorizationEndpoint] = issuerUri + "/connect/authorize";
            result[OidcConstants.Discovery.EndSessionEndpoint] = issuerUri + "/connect/endsession";
                            
            return result;
        }
    }
}