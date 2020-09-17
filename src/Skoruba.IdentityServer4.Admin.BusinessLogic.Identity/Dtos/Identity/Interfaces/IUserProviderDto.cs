namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces
{
    public interface IUserProviderDto : IBaseUserProviderDto
    {
        string Email { get; set; }
        string ProviderKey { get; set; }
        string LoginProvider { get; set; }
        string ProviderDisplayName { get; set; }
    }
}
