using System.Threading;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers.ADServices
{
    public interface IADUserSynchronizer
    {
        Task<UsersSynchronizationResult> SynchronizeAll(CancellationToken cancellationToken = default);
    }
}