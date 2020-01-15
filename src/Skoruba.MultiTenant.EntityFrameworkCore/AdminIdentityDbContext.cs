//    Copyright 2018 Andrew White
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System.Threading;
using System.Threading.Tasks;
using Skoruba.MultiTenant.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Finbuckle.MultiTenant;

namespace Skoruba.MultiTenant
{
    //public class AdminIdentityDbContext : IdentityDbContext<UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>, IMultiTenantDbContext
    //{
    //    public TenantInfo TenantInfo { get; }
    //    public TenantMismatchMode TenantMismatchMode { get; set; } = TenantMismatchMode.Throw;
    //    public TenantNotSetMode TenantNotSetMode { get; set; } = TenantNotSetMode.Throw;


    //    public AdminIdentityDbContext(DbContextOptions<AdminIdentityDbContext> options) : base(options)
    //    {

    //    }
    //    protected AdminIdentityDbContext(TenantInfo tenantInfo)
    //    {
    //        this.TenantInfo = tenantInfo;
    //    }

    //    protected AdminIdentityDbContext(TenantInfo tenantInfo, DbContextOptions options) : base(options)
    //    {
    //        this.TenantInfo = tenantInfo;
    //    }

    //    protected override void OnModelCreating(ModelBuilder builder)
    //    {
    //        base.OnModelCreating(builder);

    //        ConfigureIdentityContext(builder);
    //    }

    //    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    //    {
    //        if (this.TenantInfo != null)
    //        {
    //            this.EnforceMultiTenant();
    //        }
    //        return base.SaveChanges(acceptAllChangesOnSuccess);
    //    }

    //    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
    //                                                     CancellationToken cancellationToken = default(CancellationToken))
    //    {
    //        if (this.TenantInfo != null)
    //        {
    //            this.EnforceMultiTenant();
    //        }
    //        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    //    }

    //    private void ConfigureIdentityContext(ModelBuilder builder)
    //    {
    //        builder.Entity<UserIdentityRole>().ToTable(TableConsts.IdentityRoles);
    //        builder.Entity<UserIdentityRoleClaim>().ToTable(TableConsts.IdentityRoleClaims);
    //        builder.Entity<UserIdentityUserRole>().ToTable(TableConsts.IdentityUserRoles);

    //        builder.Entity<UserIdentity>().ToTable(TableConsts.IdentityUsers);
    //        builder.Entity<UserIdentityUserLogin>().ToTable(TableConsts.IdentityUserLogins);
    //        builder.Entity<UserIdentityUserClaim>().ToTable(TableConsts.IdentityUserClaims);
    //        builder.Entity<UserIdentityUserToken>().ToTable(TableConsts.IdentityUserTokens);

    //        builder.Entity<UserIdentityRole>().IsMultiTenant();
    //        builder.Entity<UserIdentityRoleClaim>().IsMultiTenant();
    //        builder.Entity<UserIdentityUserRole>().IsMultiTenant();
    //        builder.Entity<UserIdentity>().IsMultiTenant();
    //        builder.Entity<UserIdentityUserLogin>().IsMultiTenant();
    //        builder.Entity<UserIdentityUserClaim>().IsMultiTenant();
    //        builder.Entity<UserIdentityUserToken>().IsMultiTenant();
    //    }
    //}

}