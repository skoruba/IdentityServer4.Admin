namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces
{
    public interface IUserChangePasswordDto : IBaseUserChangePasswordDto
    {
        string Email { get; set; }
        string Password { get; set; }
        string ConfirmPassword { get; set; }
    }
}
