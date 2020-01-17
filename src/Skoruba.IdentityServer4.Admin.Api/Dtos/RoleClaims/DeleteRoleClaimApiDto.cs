using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.Api.Dtos.Users
{
    public class DeleteRoleClaimApiDto<TRoleDtoKey>
    {
        public List<TRoleDtoKey> RoleIds { get; set; }

        [Required]
        public string RoleClaimType { get; set; }
    }
}