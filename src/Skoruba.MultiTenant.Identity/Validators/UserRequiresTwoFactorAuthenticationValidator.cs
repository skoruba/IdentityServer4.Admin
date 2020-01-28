using Microsoft.AspNetCore.Identity;
using Skoruba.MultiTenant.Abstractions;
using Skoruba.MultiTenant.Configuration;
using System.Threading.Tasks;

namespace Skoruba.MultiTenant.Identity.Validators
{
    public class UserRequiresTwoFactorAuthenticationValidator<TUser> : IUserValidator<TUser>
        where TUser : class
    {
        private readonly ISkorubaTenant _skorubaTenant;

        public UserRequiresTwoFactorAuthenticationValidator(ISkorubaTenant skorubaTenant)
        {
            _skorubaTenant = skorubaTenant;
        }

        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            if (user is IdentityUser)
            {
                IdentityUser identityUser = user as IdentityUser;

                if (identityUser.TwoFactorEnabled)
                {
                    return Task.FromResult(IdentityResult.Success);
                }

                if (_skorubaTenant.TenantResolutionRequired && !_skorubaTenant.TenantResolved)
                {
                    throw MultiTenantException.MissingTenant;
                }


#if DEBUG
                // Ignore seeded email
                if (identityUser.Email.EndsWith("@skoruba.com"))
                {
                    return Task.FromResult(IdentityResult.Success);
                }
#endif

                if (_skorubaTenant.Items.TryGetValue(MultiTenantConstants.RequiresTwoFactorAuthentication, out var isrequired))
                {
                    bool requires2fa = false;
                    if (!bool.TryParse((string)isrequired, out requires2fa))
                    {
                        requires2fa = (string)isrequired == "1";
                    }

                    if (requires2fa && !identityUser.TwoFactorEnabled)
                    {
                        return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "2fa_Required", Description = "Two factor authentication is required." }));
                    }
                }
            }
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
