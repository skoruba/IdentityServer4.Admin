namespace Skoruba.MultiTenant.Configuration
{
    public class MultiTenantConfiguration
    {
        public bool MultiTenantEnabled { get; set; }
        public bool UseTenantCode { get; set; }
        public int TenantStoreCacheHours { get; set; }
    }
}