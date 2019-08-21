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
        WindowsIdentity LogonWindowsUser(string username, string password, string domain = null);
    }
}
