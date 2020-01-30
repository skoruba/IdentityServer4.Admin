using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using Skoruba.MultiTenant.Abstractions;
using Skoruba.MultiTenant.Configuration;

namespace Skoruba.MultiTenant.Finbuckle
{
    public class SkorubaTenantContext : ISkorubaTenantContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TenantInfo _tenantInfo;
        private readonly ValidateTenantRequirement _validateTenantRequirement;

        public SkorubaTenantContext(
            IHttpContextAccessor httpContextAccessor,
            TenantInfo tenantInfo,
            MultiTenantConfiguration multiTenantConfiguration,
            ValidateTenantRequirement validateTenantRequirement = null)
        {
            _httpContextAccessor = httpContextAccessor;
            _tenantInfo = tenantInfo;
            _validateTenantRequirement = validateTenantRequirement;
            MultiTenantConfiguration = multiTenantConfiguration;
        }

        public ISkorubaTenant Tenant => new SkorubaTenant(_tenantInfo ?? _httpContextAccessor?.HttpContext?.GetMultiTenantContext()?.TenantInfo);
        public bool MultiTenantEnabled => MultiTenantConfiguration?.MultiTenantEnabled ?? false;
        public bool TenantResolved => !string.IsNullOrWhiteSpace(Tenant?.Id);
        public bool TenantResolutionRequired => _validateTenantRequirement?.TenantIsRequired() ?? true;
        public string TenantResolutionStrategy => _httpContextAccessor.HttpContext?.GetMultiTenantContext()?.StrategyInfo?.StrategyType?.Name ?? "Unknown";
        public MultiTenantConfiguration MultiTenantConfiguration { get; }
    }
}