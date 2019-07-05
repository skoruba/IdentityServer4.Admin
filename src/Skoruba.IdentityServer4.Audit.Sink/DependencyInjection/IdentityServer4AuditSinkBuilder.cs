using Microsoft.Extensions.DependencyInjection;

namespace Skoruba.IdentityServer4.Audit.Sink.DependencyInjection
{
    public class IdentityServer4AuditSinkBuilder : IIdentityServer4AuditSinkBuilder
    {
        public IdentityServer4AuditSinkBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}