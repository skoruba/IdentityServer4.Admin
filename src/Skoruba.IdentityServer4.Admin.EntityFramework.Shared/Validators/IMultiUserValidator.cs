using Microsoft.AspNetCore.Identity;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Validators
{
    public interface IMultiUserValidator<TUser> : IUserValidator<TUser> where TUser : class
    {
    }
}