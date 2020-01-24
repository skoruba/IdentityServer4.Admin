using System.Collections.Generic;
using SkorubaIdentityServer4Admin.Admin.Configuration.Identity;

namespace SkorubaIdentityServer4Admin.Admin.Configuration.IdentityServer
{
    public class Client : global::IdentityServer4.Models.Client
    {
        public List<Claim> ClientClaims { get; set; } = new List<Claim>();
    }
}






