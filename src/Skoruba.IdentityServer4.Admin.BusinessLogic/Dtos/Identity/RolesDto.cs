using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity
{
    public class RolesDto
    {
        public RolesDto()
        {
            Roles = new List<RoleDto>();
        }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public List<RoleDto> Roles { get; set; }
    }
}