using Microsoft.Extensions.Options;

namespace Skoruba.IdentityServer4.Admin.Common.Settings
{
    public class SettingsRoot : ISettingsRoot
    {
        public SettingsRoot(IOptions<AdminAppSettings> appSettings)
        {
            AppSettings = appSettings.Value;
        }

        public IAdminAppSettings AppSettings { get; set; }
    }
}