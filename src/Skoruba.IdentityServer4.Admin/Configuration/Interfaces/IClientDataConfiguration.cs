using IdentityServer4.Models;
using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.Configuration.Interfaces
{
    public interface IClientDataConfiguration
    {
        List<Client> Clients { get; set; }
        List<IdentityResource> IdentityResources { get; set; }
        List<ApiResource> ApiResources { get; set; }
    }
}
