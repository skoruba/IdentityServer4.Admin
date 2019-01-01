// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace SkorubaIdentityServer4Admin.STS.Identity.Quickstart.Account
{
    public class LoggedOutViewModel
    {
        public string PostLogoutRedirectUri { get; set; }
        public string ClientName { get; set; }
        public string SignOutIframeUrl { get; set; }

        public bool AutomaticRedirectAfterSignOut { get; set; } = false;

        public string LogoutId { get; set; }
        public bool TriggerExternalSignout => ExternalAuthenticationScheme != null;
        public string ExternalAuthenticationScheme { get; set; }
    }
}