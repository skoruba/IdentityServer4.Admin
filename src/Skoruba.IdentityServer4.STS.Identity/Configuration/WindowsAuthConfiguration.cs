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

        // At each Windows login, keep the user profile in sync with any changes made to the AD
        public bool SyncUserProfileWithWindows { get; set; } = false;
        
        public bool BackgroundSynchronization { get; set; } = true;

        public TimeSpan BackgroundSynchronizationSleep { get; set; } = TimeSpan.FromMinutes(30);

        public List<ActiveDirectoryDomainConfiguration> Domains { get; set; } = new List<ActiveDirectoryDomainConfiguration>();
    }
}
