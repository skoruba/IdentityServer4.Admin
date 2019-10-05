using Skoruba.IdentityServer4.Admin.Configuration.Identity;
using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.Configuration.Interfaces
{
    public interface IIdentityDataConfiguration
    {
        List<Role> Roles { get; }
        List<User> Users { get;  }
    }
}
