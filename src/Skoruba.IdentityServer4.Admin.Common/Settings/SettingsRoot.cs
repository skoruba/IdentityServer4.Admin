using Microsoft.Extensions.Options;

namespace Skoruba.IdentityServer4.Admin.Common.Settings
{
    public class SettingsRoot : ISettingsRoot
    {
        public SettingsRoot(
            IOptions<AdminAppSettings> appSettings,
            IOptions<LoggingSettings> logging
            )
        {
            AppSettings = appSettings.Value;
            Logging = logging.Value;
        }

        public IAdminAppSettings AppSettings { get; set; }
        public ILoggingSettings Logging { get; set; }
    }
}