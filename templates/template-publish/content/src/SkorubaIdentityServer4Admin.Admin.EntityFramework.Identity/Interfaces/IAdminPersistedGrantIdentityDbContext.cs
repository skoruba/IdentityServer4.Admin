using System;
using IdentityServer4.EntityFramework.Interfaces;

namespace SkorubaIdentityServer4Admin.Admin.EntityFramework.Identity.Interfaces
{
    [Obsolete("Implement IAdminPersistedGrantDbContext instead.")]
    public interface IAdminPersistedGrantIdentityDbContext : IPersistedGrantDbContext
    {
    }
}
