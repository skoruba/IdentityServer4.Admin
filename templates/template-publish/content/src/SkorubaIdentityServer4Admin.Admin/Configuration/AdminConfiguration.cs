namespace SkorubaIdentityServer4Admin.Admin.Configuration
{
    public class AdminConfiguration
    {
        public string PageTitle { get; set; }
        public string FaviconUri { get; set; }
        public string IdentityAdminRedirectUri { get; set; }
        public string[] Scopes { get; set; }
        public string AdministrationRole { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public string IdentityAdminCookieName { get; set; }
        public double IdentityAdminCookieExpiresUtcHours { get; set; }
        public string TokenValidationClaimName { get; set; }
        public string TokenValidationClaimRole { get; set; }
        public string IdentityServerBaseUrl { get; set; }
        public string ClientId { get; set; } 
        public string ClientSecret { get; set; }
        public string OidcResponseType { get; set; }
        public bool HideUIForMSSqlErrorLogging { get; set; }
    }
}






