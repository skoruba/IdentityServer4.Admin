using Microsoft.Extensions.DependencyInjection;

namespace Skoruba.IdentityServer4.DependencyInjection
{
    /// <summary>
    /// Skoruba IdentityServer builder Interface
    /// </summary>
    public interface ISkorubaIdentityServerAdminBuilder
    {
        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>
        /// The services.
        /// </value>
        IServiceCollection Services { get; }
    }
}