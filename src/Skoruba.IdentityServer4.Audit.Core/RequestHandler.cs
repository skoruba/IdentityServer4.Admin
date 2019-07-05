using MediatR;
using System.Security.Principal;

namespace Skoruba.IdentityServer4.Audit.Core
{
    public class RequestWrapper<TRequest, TResponse> : IRequest<TResponse>
    {
        public TRequest Request { get; }
        public IPrincipal User { get; }

        public RequestWrapper(TRequest request, IPrincipal user)
        {
            Request = request;
            User = user;
        }
    }
}