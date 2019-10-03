using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class WindowsAuthConfiguration
    {
        // When integrated Windows authentication is enabled, automatically prefer it to login when request is coming from same domain
        public bool AutomaticWindowsLogin { get; set; } = false;

        // If specified, users will be searched under this OU, otherwise the RootDSE will be used
        public string WindowsUsersOURoot { get; set; } = null;

        // if user uses Windows authentication, should we load the groups from Windows
        public bool IncludeWindowsGroups { get; set; } = false;

        // if we load the groups from Windows, load only the groups that start with this prefix
        public string WindowsGroupsPrefix { get; set; } = null;

        // if we load the groups from Windows, load only the groups under this OU.
        // It is possible to specify multiple OUs separating them by a pipe character ('|')
        public string WindowsGroupsOURoot { get; set; } = null;

        // At each Windows login, keep the user profile in sync with any changes made to the AD
        public bool SyncUserProfileWithWindows { get; set; } = false;

        // If the account running the Identity Server is not a domain user, use these credentials
        // in order to access the Active Directory
        public string DomainUserName { get; set; } = null;
        public string DomainUserPassword { get; set; } = null;

        public bool GetPhotoThumbnailFromAD { get; set; } = false;
    }
}
