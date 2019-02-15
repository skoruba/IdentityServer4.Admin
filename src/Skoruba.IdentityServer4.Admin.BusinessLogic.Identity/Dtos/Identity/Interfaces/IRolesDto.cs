using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces
{
    public interface IRolesDto
    {
        int PageSize { get; set; }
        int TotalCount { get; set; }
        List<IRoleDto> Roles { get; }
    }
}
