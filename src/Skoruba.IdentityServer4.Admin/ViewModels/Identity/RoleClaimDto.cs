using System.ComponentModel.DataAnnotations;
using Skoruba.IdentityServer4.Admin.ViewModels.Identity.Base;

namespace Skoruba.IdentityServer4.Admin.ViewModels.Identity
{
    public class RoleClaimDto : BaseRoleClaimDto<int, int>
    {
        [Required]
        public string ClaimType { get; set; }

        [Required]
        public string ClaimValue { get; set; }
    }
}
