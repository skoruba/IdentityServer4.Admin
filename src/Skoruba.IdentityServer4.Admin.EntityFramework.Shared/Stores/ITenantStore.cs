using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Tenants;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Stores
{
    public interface ITenantStore : IDisposable
    {
        //
        // Summary:
        //     Creates a new tenant in a store as an asynchronous operation.
        //
        // Parameters:
        //   tenant:
        //     The tenant to create in the store.
        //
        //   cancellationToken:
        //     The System.Threading.CancellationToken used to propagate notifications that the
        //     operation should be canceled.
        //
        // Returns:
        //     A System.Threading.Tasks.Task`1 that represents the Microsoft.AspNetCore.Identity.IdentityResult
        //     of the asynchronous query.
        Task CreateAsync(Tenant tenant, CancellationToken cancellationToken);

        //
        // Summary:
        //     Deletes a tenant from the store as an asynchronous operation.
        //
        // Parameters:
        //   tenant:
        //     The tenant to delete from the store.
        //
        //   cancellationToken:
        //     The System.Threading.CancellationToken used to propagate notifications that the
        //     operation should be canceled.
        //
        // Returns:
        //     A System.Threading.Tasks.Task`1 that represents the Microsoft.AspNetCore.Identity.IdentityResult
        //     of the asynchronous query.
        Task DeleteAsync(Tenant tenant, CancellationToken cancellationToken);

        //
        // Summary:
        //     Finds the tenant who has the specified ID as an asynchronous operation.
        //
        // Parameters:
        //   tenantId:
        //     The tenant ID to look for.
        //
        //   cancellationToken:
        //     The System.Threading.CancellationToken used to propagate notifications that the
        //     operation should be canceled.
        //
        // Returns:
        //     A System.Threading.Tasks.Task`1 that result of the look up.
        Task<Tenant> FindByIdAsync(string tenantId, CancellationToken cancellationToken);

        Task<Tenant> FindByIdFromCacheAsync(string tenantId);

        //
        // Summary:
        //     Finds the tenant who has the specified normalized name as an asynchronous operation.
        //
        // Parameters:
        //   normalizedtenantName:
        //     The normalized tenant name to look for.
        //
        //   cancellationToken:
        //     The System.Threading.CancellationToken used to propagate notifications that the
        //     operation should be canceled.
        //
        // Returns:
        //     A System.Threading.Tasks.Task`1 that result of the look up.
        Task<Tenant> FindByNameAsync(string normalizedtenantName, CancellationToken cancellationToken);

        //
        // Summary:
        //     Finds the tenant who has the specified code as an asynchronous operation.
        //
        // Parameters:
        //   tenantCode:
        //     The tenant code to look for.
        //
        //   cancellationToken:
        //     The System.Threading.CancellationToken used to propagate notifications that the
        //     operation should be canceled.
        //
        // Returns:
        //     A System.Threading.Tasks.Task`1 that result of the look up.
        Task<Tenant> FindByCodeAsync(string tenantCode, CancellationToken cancellationToken);

        //
        // Summary:
        //     Updates a tenant in a store as an asynchronous operation.
        //
        // Parameters:
        //   tenant:
        //     The tenant to update in the store.
        //
        //   cancellationToken:
        //     The System.Threading.CancellationToken used to propagate notifications that the
        //     operation should be canceled.
        //
        // Returns:
        //     A System.Threading.Tasks.Task`1 that represents the Microsoft.AspNetCore.Identity.IdentityResult
        //     of the asynchronous query.
        Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken);

        /// <summary>
        /// IQueryable Tenants
        /// </summary>
        IQueryable<Tenant> Tenants { get; }
    }
}