using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces
{
    public interface IBaseUserRolesDto
    {
        object UserId { get; }
        object RoleId { get; }
    }
}
