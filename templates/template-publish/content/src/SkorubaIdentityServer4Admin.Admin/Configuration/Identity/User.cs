using System.Collections.Generic;

namespace SkorubaIdentityServer4Admin.Admin.Configuration.Identity
{
    public class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Claim> Claims { get; set; } = new List<Claim>();
        public List<string> Roles { get; set; } = new List<string>();
    }
}






