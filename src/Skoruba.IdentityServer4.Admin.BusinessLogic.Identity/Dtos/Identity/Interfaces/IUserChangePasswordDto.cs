using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces
{
    public interface IUserChangePasswordDto : IBaseUserChangePasswordDto
    {
        string UserName { get; set; }
        string Password { get; set; }
        string ConfirmPassword { get; set; }
    }
}
