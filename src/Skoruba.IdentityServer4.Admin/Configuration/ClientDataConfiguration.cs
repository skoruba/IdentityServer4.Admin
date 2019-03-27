using IdentityServer4.Models;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;
using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
    public class ClientDataConfiguration : IClientDataConfiguration
    {
        public List<Client> Clients { get; set; } = new List<Client>();
        public List<IdentityResource> IdentityResources { get; set; } = new List<IdentityResource>();
        public List<ApiResource> ApiResources { get; set; } = new List<ApiResource>();
    }
}
