using System.ComponentModel.DataAnnotations;

namespace SkorubaIdentityServer4Admin.Admin.Api.Dtos.Roles
{
    public class RoleClaimApiDto<TKey>
    {
        public int ClaimId { get; set; }

        public TKey RoleId { get; set; }

        [Required]
        public string ClaimType { get; set; }


        [Required]
        public string ClaimValue { get; set; }
    }
}





