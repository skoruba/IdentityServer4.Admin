using IdentityServer4.Models;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;
using System.Collections.Generic;
using Client = Skoruba.IdentityServer4.Admin.Configuration.IdentityServer.Client;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
    public class IdentityServerDataConfiguration : IIdentityServerDataConfiguration
    {
        public List<Client> Clients { get; set; } = new List<Client>();
        public List<IdentityResource> IdentityResources { get; set; } = new List<IdentityResource>();
        public List<ApiResource> ApiResources { get; set; } = new List<ApiResource>();
    }
}
