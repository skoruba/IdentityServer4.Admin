using Skoruba.IdentityServer4.Admin.UI.Configuration.Identity;
using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.UI.Configuration
{
    public class IdentityDataConfiguration
    {
       public List<Role> Roles { get; set; }
       public List<User> Users { get; set; }
    }
}
