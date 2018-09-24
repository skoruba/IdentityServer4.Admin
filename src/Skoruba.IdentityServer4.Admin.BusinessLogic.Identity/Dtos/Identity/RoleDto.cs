using System.ComponentModel.DataAnnotations;
using Skoruba.IdentityServer4.Admin.AspNetIdentity.Dtos.Identity.Base;

namespace Skoruba.IdentityServer4.Admin.AspNetIdentity.Dtos.Identity
{
    public class RoleDto<TKey> : BaseRoleDto<TKey>
    {      
        [Required]
        public string Name { get; set; }
    }
}