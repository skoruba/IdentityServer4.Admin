using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.Configuration.SeedModels
{
    public class User : global::Microsoft.AspNetCore.Identity.IdentityUser
    {
        public string Password { get; set; }
        public List<Claim> Claims { get; set; } = new List<Claim>();
        public List<string> Roles { get; set; } = new List<string>();
        public string TenantId { get; set; }
        public string ApplicationId { get; set; }
    }
}