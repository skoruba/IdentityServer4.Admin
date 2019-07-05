using AutoMapper;
using MediatR;
using Skoruba.IdentityServer4.Audit.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Audit.EntityFramework.MediatR
{
    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<RequestWrapper<TRequest, TResponse>, TResponse>
    {
        protected AuditDbContext DbContext { get; }
        protected IMapper Mapper { get; }

        protected RequestHandler(AuditDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext;
            Mapper = mapper;
        }

        public abstract Task<TResponse> Handle(RequestWrapper<TRequest, TResponse> request, CancellationToken cancellationToken);
    }
}