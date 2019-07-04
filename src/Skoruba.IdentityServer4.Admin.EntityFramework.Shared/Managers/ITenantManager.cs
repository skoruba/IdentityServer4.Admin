using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Tenants;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Managers
{
    public interface ITenantManager : IDisposable
    {
        ILogger Logger { get; set; }
        IQueryable<Tenant> Tenants { get; }

        Task<IdentityResult> CreateAsync(Tenant tenant, CancellationToken cancellationToken = default);

        Task<IdentityResult> DeleteAsync(Tenant tenant, CancellationToken cancellationToken = default);

        Task<Tenant> FindByIdAsync(string tenantId, CancellationToken cancellationToken = default);

        Task<Tenant> FindByIdFromCacheAsync(string tenantId);

        Task<Tenant> FindByNameAsync(string tenantName, CancellationToken cancellationToken = default);

        Task<Tenant> FindByCodeAsync(string tenantCode, CancellationToken cancellationToken = default);

        Task<bool> TenantNameExists(string tenantName, CancellationToken cancellationToken = default);

        Task<bool> TenantIdExistsAsync(string tenantId, CancellationToken cancellationToken = default);

        Task<bool> TenantCodeExistsAsync(string tenantCode, CancellationToken cancellationToken = default);

        Task<IdentityResult> UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);

        Task<bool> IsTwoFactorAuthenticationRequiredAsync(string tenantId, CancellationToken cancellationToken = default);
    }
}