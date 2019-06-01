using IdentityServer4.Models;
using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.Configuration.Interfaces
{
    public interface IClientDataConfiguration
    {
        List<Skoruba.IdentityServer4.Admin.Configuration.Identity.Client> Clients { get; }
        List<IdentityResource> IdentityResources { get; }
        List<ApiResource> ApiResources { get;  }
    }
}
