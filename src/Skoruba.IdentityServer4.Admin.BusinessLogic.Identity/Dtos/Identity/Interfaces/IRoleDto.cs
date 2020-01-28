using Skoruba.MultiTenant.Abstractions;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces
{
    public interface IRoleDto : IBaseRoleDto, IHaveTenantId
    {
        string Name { get; set; }
    }
}
