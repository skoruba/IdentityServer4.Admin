using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Skoruba.IdentityServer4.Admin.ViewModels.Identity.Base;

namespace Skoruba.IdentityServer4.Admin.ViewModels.Identity
{
    public class UserRolesDto : BaseUserRolesDto<int, int>
    {
        public UserRolesDto()
        {
           Roles = new List<RoleDto>(); 
        }
        
        public SelectList RolesList { get; set; }

        public List<RoleDto> Roles { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }
    }
}
