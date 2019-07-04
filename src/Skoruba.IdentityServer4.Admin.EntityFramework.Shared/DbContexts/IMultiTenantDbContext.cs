using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Tenants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts
{
    public interface IMultiTenantDbContext
    {
        DbSet<Tenant> Tenants { get; set; }
    }
}