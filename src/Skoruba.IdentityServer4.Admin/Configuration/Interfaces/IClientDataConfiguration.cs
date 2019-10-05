using IdentityServer4.Models;
using System.Collections.Generic;
using Client = Skoruba.IdentityServer4.Admin.Configuration.IdentityServer.Client;

namespace Skoruba.IdentityServer4.Admin.Configuration.Interfaces
{
    public interface IClientDataConfiguration
    {
        List<Client> Clients { get; }
        List<IdentityResource> IdentityResources { get; }
        List<ApiResource> ApiResources { get;  }
    }
}
