using Microsoft.EntityFrameworkCore;
using IdentityServer4.Admin.EntityFramework.Entities;

namespace IdentityServer4.Admin.EntityFramework.Interfaces
{
    public interface IAdminLogDbContext
    {
        DbSet<Log> Logs { get; set; }
    }
}
