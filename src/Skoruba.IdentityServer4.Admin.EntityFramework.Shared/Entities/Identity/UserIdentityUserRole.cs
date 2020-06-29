﻿using IdentityServer4.Admin.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using System;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity
{
    public class UserIdentityUserRole : IdentityUserRole<string>, IMultiTenant
    {
        public Guid? TenantId { get; protected set; }
    }
}