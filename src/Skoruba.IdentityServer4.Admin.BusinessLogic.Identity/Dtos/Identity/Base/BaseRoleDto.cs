using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces;
using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Base
{
    public class BaseRoleDto<TRoleId> : IBaseRoleDto
    {
        public TRoleId Id { get; set; }

        public bool IsDefaultId() => EqualityComparer<TRoleId>.Default.Equals(Id, default(TRoleId));

        object IBaseRoleDto.Id => Id;
    }
}