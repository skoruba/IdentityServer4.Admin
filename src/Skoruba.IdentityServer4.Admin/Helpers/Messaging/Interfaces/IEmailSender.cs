#region Assembly Microsoft.AspNetCore.Identity.UI, Version=3.1.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60
// Microsoft.AspNetCore.Identity.UI.dll
#endregion

using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.Helpers.Messaging.Interfaces
{
    // Note:
    // This is not particularly required, but prevents a new library from being included in this project
    // Originall taken from Microsoft.AspNetCore.Identity.UI.Services
    // 
    // Summary:
    //     This API supports the ASP.NET Core Identity default UI infrastructure and is
    //     not intended to be used directly from your code. This API may change or be removed
    //     in future releases.
    public interface IEmailSender
    {
        //
        // Summary:
        //     This API supports the ASP.NET Core Identity default UI infrastructure and is
        //     not intended to be used directly from your code. This API may change or be removed
        //     in future releases.
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}