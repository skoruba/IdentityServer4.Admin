using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.Api.Dtos.Users
{
    public class CreateRoleClaimApiDto<TRoleDtoKey>
    {
        public List<TRoleDtoKey> RoleIds { get; set; }

        [Required]
        public string RoleClaimType { get; set; }

        [Required]
        public string RoleClaimValue { get; set; }
    }
}