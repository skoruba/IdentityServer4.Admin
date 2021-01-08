using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.UI.Configuration.Identity
{
    public class Role
    {
        public string Name { get; set; }
        public List<Claim> Claims { get; set; } = new List<Claim>();
    }
}
