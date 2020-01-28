using Microsoft.AspNetCore.Identity;
using Skoruba.MultiTenant.Abstractions;
using System.Threading.Tasks;

namespace Skoruba.MultiTenant.Identity.Validators
{
    public class RoleRequiresTenantIdValidator<TRole> : IRoleValidator<TRole>
        where TRole : class
    {
        private readonly ISkorubaTenant _skorubaTenant;

        public RoleRequiresTenantIdValidator(ISkorubaTenant skorubaTenant)
        {
            _skorubaTenant = skorubaTenant;
        }

        public Task<IdentityResult> ValidateAsync(RoleManager<TRole> manager, TRole role)
        {
            if (_skorubaTenant.TenantResolutionRequired && !_skorubaTenant.TenantResolved)
            {
                throw MultiTenantException.MissingTenant;
            }
            if (string.IsNullOrEmpty(((IHaveTenantId)role).TenantId))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "Tenant_Required", Description = "A tenant id is required." }));
            }
            if (((IHaveTenantId)role).TenantId != _skorubaTenant.Id)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "Tenant_Invalid", Description = "The tenant id must be the same as the current user's tenant id." }));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
