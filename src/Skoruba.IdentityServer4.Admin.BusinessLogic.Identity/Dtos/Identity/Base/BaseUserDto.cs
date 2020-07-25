using IdentityServer4.Admin.MultiTenancy;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces;
using System;
using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Base
{
    public class BaseUserDto<TUserId> : IBaseUserDto, IMultiTenant
    {
        public TUserId Id { get; set; }

        public bool IsDefaultId() => EqualityComparer<TUserId>.Default.Equals(Id, default(TUserId));

        object IBaseUserDto.Id => Id;

        public Guid? TenantId { get; set; }
    }
}