using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using Skoruba.MultiTenant.Abstractions;
using Skoruba.MultiTenant.Configuration;

namespace Skoruba.MultiTenant.Finbuckle
{
    public class SkorubaTenantContext : ISkorubaTenantContext
    {
        private SkorubaTenantContext(
            TenantInfo tenantInfo,
            string resolutionStrategy,
            MultiTenantConfiguration multiTenantConfiguration,
            ValidateTenantRequirement validateTenantRequirement)
        {
            Tenant = new SkorubaTenant(tenantInfo);

            TenantResolutionStrategy = resolutionStrategy ?? "Unknown";

            TenantResolutionRequired = validateTenantRequirement?.TenantIsRequired() ?? multiTenantConfiguration.MultiTenantEnabled;

            MultiTenantConfiguration = multiTenantConfiguration;
        }
        public SkorubaTenantContext(
            IHttpContextAccessor httpContextAccessor,
            MultiTenantConfiguration multiTenantConfiguration,
            ValidateTenantRequirement validateTenantRequirement = null)
            : this(
                httpContextAccessor.HttpContext?.GetMultiTenantContext()?.TenantInfo,
                httpContextAccessor.HttpContext?.GetMultiTenantContext()?.StrategyInfo?.StrategyType?.Name,
                multiTenantConfiguration,
                validateTenantRequirement
            )
        { }
        public SkorubaTenantContext(
            IHttpContextAccessor httpContextAccessor,
            TenantInfo tenantInfo,
            MultiTenantConfiguration multiTenantConfiguration,
            ValidateTenantRequirement validateTenantRequirement = null)
            : this(
                tenantInfo ?? httpContextAccessor.HttpContext?.GetMultiTenantContext()?.TenantInfo,
                httpContextAccessor.HttpContext?.GetMultiTenantContext()?.StrategyInfo?.StrategyType?.Name,
                multiTenantConfiguration,
                validateTenantRequirement
            )
        { }

        public ISkorubaT Tenant { get; }
        public bool MultiTenantEnabled => MultiTenantConfiguration?.MultiTenantEnabled ?? false;
        public bool TenantResolved => !string.IsNullOrWhiteSpace(Tenant?.Id);
        public bool TenantResolutionRequired { get; }
        public string TenantResolutionStrategy { get; }
        public MultiTenantConfiguration MultiTenantConfiguration { get; }
    }
}