namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class LoginConfiguration
    {
        public LoginResolutionPolicy ResolutionPolicy { get; set; } = LoginResolutionPolicy.Username;

        // When integrated Windows authentication is enabled, automatically prefer it to login when request is coming from same domain
        public bool AutomaticWindowsLogin { get; set; } = false;

        // if user uses Windows authentication, should we load the groups from Windows
        public bool IncludeWindowsGroups { get; set; } = false;

        // if we load the groups from Windows, load only the groups that start with this prefix
        public string WindowsGroupsPrefix { get; set; } = null;

        // if we load the groups from Windows, load only the groups under this OU.
        // It is possible to specify multiple OUs separating them by a pipe character ('|')
        public string WindowsGroupsOURoot { get; set; } = null;
    }
}
