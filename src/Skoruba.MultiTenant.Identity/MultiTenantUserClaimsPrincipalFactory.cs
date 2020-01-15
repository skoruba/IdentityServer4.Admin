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
    public class MultiTenantUserClaimsPrincipalFactory<TUser, TUserRole> : UserClaimsPrincipalFactory<TUser, TUserRole> where TUser : class where TUserRole : class
    {
        private readonly ISkorubaTenant _skorubaMultiTenant;

        public MultiTenantUserClaimsPrincipalFactory(
            UserManager<TUser> userManager,
            RoleManager<TUserRole> roleManager,
            IOptions<IdentityOptions> options,
            ISkorubaTenant skorubaMultiTenant) : base(userManager, roleManager, options)
        {
            _skorubaMultiTenant = skorubaMultiTenant;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
        {
            var userclaims = await UserManager.GetClaimsAsync(user);

            //if (!_skorubaMultiTenant.TenantResolved && _skorubaMultiTenant.TenantResolutionRequired)
            //{
            //    throw new System.Exception("Tenant is required.");
            //}

            if (_skorubaMultiTenant.TenantResolved)
            {
                // validate that the tenant id is stored as a claim for the user
                // this is important becuase this storage location is where idsrv
                // retrieves claims for client requests
                if (!userclaims.Any(a => a.Type == Claims.ClaimTypes.TenantId))
                {
                    var result = await UserManager.AddClaimAsync(user, new Claim(Claims.ClaimTypes.TenantId, _skorubaMultiTenant.Id));
                }
            }

            var identity = await base.GenerateClaimsAsync(user);

            return identity;
        }
    }
}