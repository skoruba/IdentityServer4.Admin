using System.ComponentModel.DataAnnotations;
using Skoruba.IdentityServer4.Admin.AspNetIdentity.Dtos.Identity.Base;

namespace Skoruba.IdentityServer4.Admin.AspNetIdentity.Dtos.Identity
{
    public class RoleClaimDto<TRoleDtoKey> : BaseRoleClaimDto<TRoleDtoKey>
    {
        [Required]
        public string ClaimType { get; set; }

        [Required]
        public string ClaimValue { get; set; }
    }
}
