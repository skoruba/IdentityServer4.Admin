using System;
using IdentityServer4.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Interfaces
{
    [Obsolete("Implement IAdminPersistedGrantDbContext instead.")]
    public interface IAdminPersistedGrantIdentityDbContext : IPersistedGrantDbContext
    {
    }
}
