namespace Skoruba.IdentityServer4.Admin.EntityFramework.Constants
{
    public static class TableConsts
    {
        public const string Logging = "Log";
        public const string Tenant = "Tenants";
        public const string TenantConnectionString = "TenantConnectionStrings";
    }

    public static class TenantConsts
    {
        public const int MaxNameLength = 64;
    }

    public static class TenantConnectionStringConsts
    {
        public const int MaxNameLength = 64;
        public const int MaxValueLength = 1024;
    }
}
