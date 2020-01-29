using Microsoft.AspNetCore.Identity;
using Skoruba.MultiTenant.Abstractions;
using System.Threading.Tasks;

namespace Skoruba.MultiTenant.Identity.Validators
{
    public class RoleRequiresTenantIdValidator<TRole> : IRoleValidator<TRole>
        where TRole : class
    {
        private readonly ISkorubaTenantContext _skorubaTenantContext;

        public RoleRequiresTenantIdValidator(ISkorubaTenantContext skorubaTenantContext)
        {
            _skorubaTenantContext = skorubaTenantContext;
        }

        public Task<IdentityResult> ValidateAsync(RoleManager<TRole> manager, TRole role)
        {
            if (_skorubaTenantContext.TenantResolutionRequired && !_skorubaTenantContext.TenantResolved)
            {
                throw MultiTenantException.MissingTenant;
            }
            if (string.IsNullOrEmpty(((IHaveTenantId)role).TenantId))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "Tenant_Required", Description = "A tenant id is required." }));
            }
            if (((IHaveTenantId)role).TenantId != _skorubaTenantContext.Tenant.Id)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "Tenant_Invalid", Description = "The tenant id must be the same as the current user's tenant id." }));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
