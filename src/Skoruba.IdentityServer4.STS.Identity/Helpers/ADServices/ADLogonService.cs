using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers.ADServices
{
    public class ADLogonService
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String Username, String Domain, String Password, int LogonType, int LogonProvider, out SafeAccessTokenHandle Token);
        const int LOGON32_PROVIDER_DEFAULT = 0;
        //This parameter causes LogonUser to create a primary token.     
        const int LOGON32_LOGON_INTERACTIVE = 2;

        private readonly ILogger<ADLogonService> _logger;

        public ADLogonService(
            ILogger<ADLogonService> logger
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));            
        }

        public WindowsIdentity LogonWindowsUser(string username, string password, string domain = null)
        {
            if (string.IsNullOrEmpty(domain))
            {
                try
                {
                    domain = Domain.GetComputerDomain().Name;
                }
                catch (ActiveDirectoryObjectNotFoundException)
                {
                    // The computer is not part of any domain and cannot authenticate users against AD
                    return null;
                }
            }

            bool returnValue = LogonUser(username, domain, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out SafeAccessTokenHandle safeAccessTokenHandle);
            if (returnValue)
                return new WindowsIdentity(safeAccessTokenHandle.DangerousGetHandle());

            // Authentication failed
            return null;
        }
    }
}
