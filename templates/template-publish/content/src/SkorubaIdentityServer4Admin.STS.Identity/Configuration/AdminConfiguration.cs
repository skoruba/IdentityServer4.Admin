using SkorubaIdentityServer4Admin.STS.Identity.Configuration.Intefaces;

namespace SkorubaIdentityServer4Admin.STS.Identity.Configuration
{
    public class AdminConfiguration : IAdminConfiguration
    {
        public string IdentityAdminBaseUrl { get; set; } = "http://localhost:9000";
    }
}