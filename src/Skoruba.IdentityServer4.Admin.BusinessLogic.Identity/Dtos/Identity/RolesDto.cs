using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity
{
    public class RolesDto<TRoleDto, TRoleDtoKey> where TRoleDto : RoleDto<TRoleDtoKey>
    {
        public RolesDto()
        {
            Roles = new List<TRoleDto>();
        }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public List<TRoleDto> Roles { get; set; }
    }
}