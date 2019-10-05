using Skoruba.IdentityServer4.Admin.Configuration.Identity;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;
using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
    public class IdentityDataConfiguration : IIdentityDataConfiguration
    {
       public List<Role> Roles { get; set; }
       public List<User> Users { get; set; }
    }
}
