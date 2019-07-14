using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity
{
    public class MultiTenantUserDto<TKey> : UserDto<TKey>, IMultiTenantUserDto
    {
        public string TenantId { get; set; }
        public string ApplicationId { get; set; }
    }
}