using System.Threading.Tasks;

namespace IdentityServer4.Admin.MultiTenancy.Infrastructure
{
    public interface ITenantResolveContributor
    {
        string Name { get; }

        void Resolve(ITenantResolveContext context);
    }
}
