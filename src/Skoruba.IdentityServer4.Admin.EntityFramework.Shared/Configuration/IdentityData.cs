using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Configuration.Identity;
using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Configuration
{
	public class IdentityData
    {
       public List<Role> Roles { get; set; }
       public List<User> Users { get; set; }
    }
}
