﻿namespace Skoruba.IdentityServer4.Admin.Configuration
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
        public string Theme { get; set; }

        public string CustomThemeCss { get; set; }

        // EZY-modification (EZYC-3029): below our custom settings
        public bool IdentityServerUseExternalBaseUrl { get; set; }
        public string IdentityServerExternalBaseUrl { get; set; }
        public string ApplicationName { get; set; } = "Id.Admin";
    }
}
