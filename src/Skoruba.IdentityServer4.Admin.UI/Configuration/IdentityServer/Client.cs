using System.Collections.Generic;
using Skoruba.IdentityServer4.Admin.Configuration.Identity;

namespace Skoruba.IdentityServer4.Admin.Configuration.IdentityServer
{
    public class Client : global::IdentityServer4.Models.Client
    {
        public List<Claim> ClientClaims { get; set; } = new List<Claim>();
    }
}
