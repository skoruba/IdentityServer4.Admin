namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces
{
    public interface IMultiTenantUserDto : IUserDto
    {
        string TenantId { get; set; }
    }
}
