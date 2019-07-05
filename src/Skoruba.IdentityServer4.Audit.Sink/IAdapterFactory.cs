using IdentityServer4.Events;
using Microsoft.AspNetCore.Http;

namespace Skoruba.IdentityServer4.Audit.Sink
{
    public interface IAdapterFactory
    {
        IAuditArgs Create(Event evt, IHttpContextAccessor httpContext);
    }
}