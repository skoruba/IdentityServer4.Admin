﻿using Skoruba.IdentityServer4.STS.Identity.Configuration.Intefaces;

namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class AdminConfiguration : IAdminConfiguration
    {
        public string IdentityAdminBaseUrl { get; set; } = "https://localhost:44390";
    }
}