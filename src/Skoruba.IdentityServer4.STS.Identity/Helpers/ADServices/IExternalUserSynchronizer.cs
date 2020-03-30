using System.Threading;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers.ADServices
{
    public interface IExternalUserSynchronizer
    {
        string LoginProvider { get; }

        Task<UsersSynchronizationResult> SynchronizeAll(CancellationToken cancellationToken = default);
    }
}