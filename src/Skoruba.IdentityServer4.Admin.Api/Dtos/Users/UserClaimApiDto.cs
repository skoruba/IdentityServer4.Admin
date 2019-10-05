using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.Api.Dtos.Users
{
    public class UserClaimApiDto<TUserDtoKey>
    {
        public int ClaimId { get; set; }

        public TUserDtoKey UserId { get; set; }

        [Required]
        public string ClaimType { get; set; }

        [Required]
        public string ClaimValue { get; set; }
    }
}