using IdentityServer4.Models;
using System.Collections.Generic;
using Client = Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Configuration.IdentityServer.Client;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Configuration
{
    public class IdentityServerData
    {
        public List<Client> Clients { get; set; } = new List<Client>();
        public List<IdentityResource> IdentityResources { get; set; } = new List<IdentityResource>();
        public List<ApiResource> ApiResources { get; set; } = new List<ApiResource>();
        public List<ApiScope> ApiScopes { get; set; } = new List<ApiScope>();
    }
}
