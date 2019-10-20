using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers.ADUtilities
{
    public interface IADUtilities
    {
        ADProperties GetUserInfoFromAD(string userId);
        // Given a list of roles, returns only those that are Windows groups that comply with the configured policy
        IEnumerable<string> FilterADGroups(IEnumerable<string> adGroups);
        WindowsIdentity LogonWindowsUser(string username, string password, string domain = null);
    }
}
