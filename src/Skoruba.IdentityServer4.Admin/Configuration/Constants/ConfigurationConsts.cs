using System;

namespace Skoruba.IdentityServer4.Admin.Configuration.Constants
{
    public class ConfigurationConsts
    {
        [Obsolete] public const string AdminConnectionStringKey = "AdminConnection";

		[Obsolete] public const string ConfigurationDbConnectionStringKey = "ConfigurationDbConnection";

		[Obsolete] public const string PersistedGrantDbConnectionStringKey = "PersistedGrantDbConnection";

		[Obsolete] public const string IdentityDbConnectionStringKey = "IdentityDbConnection";

		[Obsolete] public const string AdminLogDbConnectionStringKey = "AdminLogDbConnection";

        public const string ResourcesPath = "Resources";

        public const string AdminConfigurationKey = "AdminConfiguration";
    }
}