using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Skoruba.MultiTenant.Abstractions;
using Skoruba.MultiTenant.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Skoruba.MultiTenant.Identity
{


    /// <summary>
    /// Creates claims for a multi tenant user ensuring that the TenantId claim is added to the user.
    /// </summary>
    /// <remarks>
    /// STS Account/Login uses the UserManager to generate an identity.  By default the UserManager
    /// will retreive claims from the user store and only get claims from the store.  
    /// </remarks>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TUserRole"></typeparam>
    public class MultiTenantUserClaimsPrincipalFactory<TUser, TUserRole> : UserClaimsPrincipalFactory<TUser, TUserRole> where TUser : class where TUserRole : class
    {
        public MultiTenantUserClaimsPrincipalFactory(
            UserManager<TUser> userManager,
            RoleManager<TUserRole> roleManager,
            IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
        { }

        /// <summary>
        /// Generate the claims for a user including TenantId.
        /// </summary>
        /// <param name="user">The user to create a <see cref="ClaimsIdentity"/> from.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous creation operation, containing the created <see cref="ClaimsIdentity"/>.</returns>
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
        {
            var id = await base.GenerateClaimsAsync(user);

            if (user is IHaveTenantId)
            {
                // get the tenantid from the user
                string tenantId = ((IHaveTenantId)user).TenantId;

                // if the tenantid is not already a user claim then we need to add it
                if (!id.Claims.Any(a => a.Type == Claims.ClaimTypes.TenantId))
                {
                    id.AddClaim(new Claim(Claims.ClaimTypes.TenantId, tenantId));
                }
            }

            return id;
        }
        
        
        //protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
        //{
        //    // only do the following if this is multi tenant
        //    if (user is IHaveTenantId)
        //    {
        //        // get the users claims from store
        //        var userclaims = await UserManager.GetClaimsAsync(user);

        //        // get the tenantid from the user
        //        string tenantId = ((IHaveTenantId)user).TenantId;

        //        // if the tenantid is not already a user claim then we need to add it
        //        if (!userclaims.Any(a => a.Type == Claims.ClaimTypes.TenantId))
        //        {
        //            // add the claim to the user store
        //            var result = await UserManager.AddClaimAsync(user, new Claim(Claims.ClaimTypes.TenantId, tenantId));
        //        }
        //    }

        //    // use the base class to generate claims
        //    return await base.GenerateClaimsAsync(user);
        //}
    }
}