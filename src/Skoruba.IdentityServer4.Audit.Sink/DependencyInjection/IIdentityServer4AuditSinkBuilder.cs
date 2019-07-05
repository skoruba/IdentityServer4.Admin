using Microsoft.Extensions.DependencyInjection;

namespace Skoruba.IdentityServer4.Audit.Sink.DependencyInjection
{
    public interface IIdentityServer4AuditSinkBuilder
    {
        IServiceCollection Services { get; }
    }
}
