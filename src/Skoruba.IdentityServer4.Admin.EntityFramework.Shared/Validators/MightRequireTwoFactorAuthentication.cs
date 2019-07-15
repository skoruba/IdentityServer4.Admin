using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Validators
{
    /// <summary>
    /// Requires the user to have TwoFactoEnabled set to true if the tenant is configured to require
    /// TwoFactorAuthentication or if the ITenantManager is not registered.  For instance, in a single
    /// tenant configuration the ITenantManager is not registered by default and will therefore
    /// require TwoFactorEnabled if this is registered.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class MightRequireTwoFactorAuthentication<TUser> : IMultiUserValidator<TUser>
        where TUser : IdentityUser
    {
        private readonly ITenantManager _tenantManager;

        public MightRequireTwoFactorAuthentication(ITenantManager tenantManagers)
        {
            _tenantManager = tenantManagers;
        }

        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            if (user.TwoFactorEnabled)
            {
                return IdentityResult.Success;
            }

#if DEBUG
            // Ignore seeded email
            if (user.Email.EndsWith("@example.com"))
            {
                return IdentityResult.Success;
            }
#endif

            // Check if required on tenant
            if (_tenantManager != null && user as MultiTenantUserIdentity != null)
            {
                if (!await _tenantManager.IsTwoFactorAuthenticationRequiredAsync((user as MultiTenantUserIdentity).TenantId))
                {
                    return IdentityResult.Success;
                }
            }

            return IdentityResult.Failed(new IdentityError { Code = "2fa_Required", Description = "Two factor authentication is required." });
        }
    }
}