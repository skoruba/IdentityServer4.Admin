using System.ComponentModel.DataAnnotations;
using IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Base;
using IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces;

namespace IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity
{
    public class UserClaimDto<TUserDtoKey> : BaseUserClaimDto<TUserDtoKey>, IUserClaimDto
    {
        [Required]
        public string ClaimType { get; set; }

        [Required]
        public string ClaimValue { get; set; }
    }
}