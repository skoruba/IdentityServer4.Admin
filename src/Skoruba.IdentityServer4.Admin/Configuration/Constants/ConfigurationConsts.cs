﻿using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.Configuration.Constants
{
    public class ConfigurationConsts
    {
        public const string AdminConnectionStringKey = "AdminConnection";

        public const string ConfigurationDbConnectionStringKey = "ConfigurationDbConnection";

        public const string PersistedGrantDbConnectionStringKey = "PersistedGrantDbConnection";

        public const string IdentityDbConnectionStringKey = "IdentityDbConnection";

        public const string AdminLogDbConnectionStringKey = "AdminLogDbConnection";

        public const string ResourcesPath = "Resources";

        public const string AdminConfigurationKey = "AdminConfiguration";

        public const string IdentityServerDataConfigurationKey = "IdentityServerData";

        public const string IdentityDataConfigurationKey = "IdentityData";

        public const string AdminAuditLogDbConnectionStringKey = "AdminAuditLogDbConnection";

        public const string DataProtectionDbConnectionStringKey = "DataProtectionDbConnection";

        public const string CspTrustedDomainsKey = "CspTrustedDomains";

        // EZY-modification (EZYC-3029): below our custom settings
        public const string SingleDbConnectionStringKey = "SingleDbConnection";
    }
}