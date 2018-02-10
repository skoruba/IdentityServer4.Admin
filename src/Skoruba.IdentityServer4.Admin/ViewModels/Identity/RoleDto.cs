using System.ComponentModel.DataAnnotations;
using Skoruba.IdentityServer4.Admin.ViewModels.Identity.Base;

namespace Skoruba.IdentityServer4.Admin.ViewModels.Identity
{
    public class RoleDto : BaseRoleDto<int>
    {      
        [Required]
        public string Name { get; set; }
    }
}