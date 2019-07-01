using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Core.MultiTenant.Identity;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Validators
{
    public class RequireTwoFactorAuthentication : IMultiUserValidator<MultiTenantUserIdentity>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<MultiTenantUserIdentity> manager, MultiTenantUserIdentity user)
        {
            if (user.TwoFactorEnabled)
            {
                return Task.FromResult(IdentityResult.Success);
            }
            // Allow for seeded user to configure email
            if (user.Email == "admin@example.com")
            {
                return Task.FromResult(IdentityResult.Success);
            }
            return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "2fa_Required", Description = "Two factor authentication is required." }));
        }
    }
}