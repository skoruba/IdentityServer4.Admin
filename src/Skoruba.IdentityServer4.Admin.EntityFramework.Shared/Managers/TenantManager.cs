using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Managers;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Stores;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Tenants;
using Microsoft.EntityFrameworkCore;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Managers
{
    public class TenantManager : ITenantManager
    {
        private bool _disposed;

        //
        // Summary:
        //     Constructs a new instance of TenantManager.
        //
        // Parameters:
        //   store:
        //     The persistence store the manager will operate over.
        //
        //   logger:
        //     The logger used to log messages, warnings and errors.
        public TenantManager(ITenantStore store, ILogger<TenantManager> logger)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            Logger = logger;
        }

        public virtual IQueryable<Tenant> Tenants => Store.Tenants;

        //
        // Summary:
        //     Gets the Microsoft.Extensions.Logging.ILogger used to log messages from the manager.
        public virtual ILogger Logger { get; set; }

        //
        // Summary:
        //     Gets the persistence store this instance operates over.
        protected ITenantStore Store { get; }

        //
        // Summary:
        //     Creates the specified tenant in the persistence store.
        //
        // Parameters:
        //   tenant:
        //     The tenant to create.
        public virtual async Task<IdentityResult> CreateAsync(Tenant tenant, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            ThrowIfArgumentNull(tenant);
            await Store.CreateAsync(tenant, cancellationToken);
            return IdentityResult.Success;
        }

        //
        // Summary:
        //     Deletes the specified tenant in the persistence store.
        //
        // Parameters:
        //   tenant:
        //     The tenant to delete.
        public virtual async Task<IdentityResult> DeleteAsync(Tenant tenant, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            ThrowIfArgumentNull(tenant);
            await Store.DeleteAsync(tenant, cancellationToken);
            return IdentityResult.Success;
        }

        //
        // Summary:
        //     Updates the specified tenant in the persistence store.
        //
        // Parameters:
        //   tenant:
        //     The tenant to update.
        public virtual async Task<IdentityResult> UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            ThrowIfArgumentNull(tenant);
            await Store.UpdateAsync(tenant, cancellationToken);
            return IdentityResult.Success;
        }

        public virtual async Task<bool> TenantNameExists(string tenantName, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            ThrowIfArgumentNull(tenantName, "tenantName");
            return await Store.FindByNameAsync(tenantName, cancellationToken) != null;
        }

        public virtual async Task<Tenant> FindByNameAsync(string tenantName, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            ThrowIfArgumentNull(tenantName, "tenantName");
            return await Store.FindByNameAsync(tenantName, cancellationToken);
        }

        public virtual async Task<Tenant> FindByCodeAsync(string tenantCode, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            ThrowIfArgumentNull(tenantCode, "tenantCode");
            return await Store.FindByCodeAsync(tenantCode, cancellationToken);
        }

        private void ThrowIfArgumentNull<T>(T tenant, string name = "tenant")
        {
            if (typeof(T) == typeof(string))
            {
                if (string.IsNullOrEmpty(tenant as string))
                {
                    throw new ArgumentNullException(name);
                }
            }
            if (tenant == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>
        ///     Dispose this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        ///     When disposing, actually dipose the store
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                Store.Dispose();
            }
            _disposed = true;
        }

        public Task<Tenant> FindByIdAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            ThrowIfArgumentNull(tenantId, "tenantId");
            return Store.FindByIdAsync(tenantId, cancellationToken);
        }

        public async Task<bool> TenantIdExistsAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            return await FindByIdAsync(tenantId, cancellationToken) != null;
        }

        public async Task<bool> TenantCodeExistsAsync(string tenantCode, CancellationToken cancellationToken = default)
        {
            return await FindByCodeAsync(tenantCode, cancellationToken) != null;
        }

        public Task<Tenant> FindByIdFromCacheAsync(string tenantId)
        {
            return Store.FindByIdFromCacheAsync(tenantId);
        }

        public async Task<bool> IsTwoFactorAuthenticationRequiredAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            return await Store.Tenants
                .Where(a => a.Id == tenantId)
                .Select(a => a.RequireTwoFactorAuthentication)
                .FirstOrDefaultAsync();
        }
    }
}