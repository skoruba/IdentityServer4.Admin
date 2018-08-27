using System;

namespace Skoruba.IdentityServer4.Admin.Common.Settings
{
    public class AdminAppSettings : IAdminAppSettings
    {
        public string AdministrationRole { get; set; } = "SkorubaIdentityAdminAdministrator";
        public string IdentityAdminCookieName { get; set; } = "IdentityServerAdmin";
        public string IdentityAdminRedirectUri { get; set; } = "http://localhost:9000/signin-oidc";
        public string IdentityServerBaseUrl { get; set; } = "http://localhost:5000";
        public string IdentityAdminBaseUrl { get; set; } = "http://localhost:9000";
        public string OidcClientId { get; set; } = "skoruba_identity_admin";
    }
}
