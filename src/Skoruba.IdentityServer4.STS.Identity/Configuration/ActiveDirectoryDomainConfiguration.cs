using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class ActiveDirectoryDomainConfiguration
    {
        public bool GetPhotoThumbnailFromAD { get; set; } = true;

        //Full qualified name of the domain
        public string DomainFullQualifiedName { get; set; } = null;

        // If the account running the Identity Server is not a domain user, use these credentials
        // in order to access the Active Directory
        public string DomainUserName { get; set; } = null;
        public string DomainUserPassword { get; set; } = null;

        // If specified, users will be searched under this OU, otherwise the RootDSE will be used
        public IEnumerable<string> WindowsUsersOURoot { get; set; } = Array.Empty<string>();

        // if user uses Windows authentication, should we load the groups from Windows
        public bool IncludeWindowsGroups { get; set; } = true;

        // if we load the groups from Windows, load only the groups that start with this prefix
        public string WindowsGroupsPrefix { get; set; } = null;

    }
}
