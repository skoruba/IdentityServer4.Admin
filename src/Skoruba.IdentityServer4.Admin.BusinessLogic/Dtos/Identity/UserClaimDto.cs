using System.ComponentModel.DataAnnotations;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity.Base;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity
{
    public class UserClaimDto : BaseUserClaimDto<int, int>
    {
        [Required]
        public string ClaimType { get; set; }

        [Required]
        public string ClaimValue { get; set; }
    }
}