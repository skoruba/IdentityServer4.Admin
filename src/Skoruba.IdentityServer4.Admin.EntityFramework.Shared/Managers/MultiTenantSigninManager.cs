using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Managers
{
    public class MultiTenantSigninManager<TUser> : SignInManager<TUser> where TUser : class
    {
        public MultiTenantSigninManager(UserManager<TUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<TUser>> logger,
            IAuthenticationSchemeProvider schemes)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
        }

        public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return Task.FromResult(SignInResult.Failed);
        }

        public async Task<SignInResult> PasswordSignInAsync(string tenantCode, string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var user = await (UserManager as MultiTenantUserManager<TUser>).FindByNameAsync(tenantCode, userName);
            if (user == null)
            {
                return SignInResult.Failed;
            }
            return await base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }
    }

    public static class SigninManagerExtensions
    {
        public static Task<SignInResult> PasswordSignInAsync<TUser>(this SignInManager<TUser> signinManager, string tenantCode, string userName, string password, bool isPersistent, bool lockoutOnFailure) where TUser : class
        {
            return (signinManager as MultiTenantSigninManager<TUser>).PasswordSignInAsync(tenantCode, userName, password, isPersistent, lockoutOnFailure);
        }
    }
}