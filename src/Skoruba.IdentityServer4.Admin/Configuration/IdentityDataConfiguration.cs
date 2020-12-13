using System.Collections.Generic;

using Skoruba.IdentityServer4.Admin.Configuration.Identity;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
    public class IdentityDataConfiguration
    {
        public List<Role> Roles { get; set; }
        public List<User> Users { get; set; }
    }
}
