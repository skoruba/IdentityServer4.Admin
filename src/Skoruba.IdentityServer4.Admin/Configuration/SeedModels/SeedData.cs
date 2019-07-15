using System.Collections.Generic;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Tenants;

namespace Skoruba.IdentityServer4.Admin.Configuration.SeedModels
{
    public interface ISeedData
    {
        List<ApiResource> ApiResources { get; }
        List<Client> Clients { get; }
        List<IdentityResource> IdentityResources { get; }
        List<Role> Roles { get; }

        // List<Tenant> Tenants { get; }
        List<User> Users { get; }
    }

    public class SeedData : ISeedData
    {
        public List<Client> Clients { get; set; }
        public List<IdentityResource> IdentityResources { get; set; }
        public List<ApiResource> ApiResources { get; set; }

        public List<Tenant> Tenants { get; set; }
        public List<Role> Roles { get; set; }

        public List<User> Users { get; set; }
    }
}