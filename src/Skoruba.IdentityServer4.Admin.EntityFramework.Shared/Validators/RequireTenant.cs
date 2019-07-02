using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Validators
{
    public class RequireTenant : IUserValidator<MultiTenantUserIdentity>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<MultiTenantUserIdentity> manager, MultiTenantUserIdentity user)
        {
            if (string.IsNullOrEmpty(user.TenantId))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "Tenant_Required", Description = "A tenant id is required." }));
            }
            return Task.FromResult(IdentityResult.Success);
        }
    }
}