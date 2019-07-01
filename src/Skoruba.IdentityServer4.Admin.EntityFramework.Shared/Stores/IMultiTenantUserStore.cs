using System.Threading;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Stores
{
    public interface IMultiTenantUserStore<TUser>
    {
        Task<TUser> FindByEmailAsync(string tenantCode, string normalizedEmail, CancellationToken cancellationToken = default);

        Task<TUser> FindByNameAsync(string tenantCode, string normalizedUserName, CancellationToken cancellationToken = default);
    }
}