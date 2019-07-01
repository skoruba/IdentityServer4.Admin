using Microsoft.Extensions.DependencyInjection;

namespace Skoruba.IdentityServer4.Admin.Api.DependencyInjection
{
    /// <summary>
    /// Skoruba IdentityServer builder Interface
    /// </summary>
    public interface IBuilder
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