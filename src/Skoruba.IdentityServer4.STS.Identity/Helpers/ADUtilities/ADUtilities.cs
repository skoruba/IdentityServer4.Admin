using Microsoft.Win32.SafeHandles;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers.ADUtilities
{
    public class ADUtilities : IADUtilities
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String Username, String Domain, String Password, int LogonType, int LogonProvider, out SafeAccessTokenHandle Token);
        const int LOGON32_PROVIDER_DEFAULT = 0;
        //This parameter causes LogonUser to create a primary token.     
        const int LOGON32_LOGON_INTERACTIVE = 2;

        protected readonly WindowsAuthConfiguration _windowsAuthConfiguration;

        protected readonly List<string> _AllowedWindowsGroups = new List<string>();
        
        public ADUtilities(WindowsAuthConfiguration windowsAuthConfiguration)
        {
            _windowsAuthConfiguration = windowsAuthConfiguration;

            if (!string.IsNullOrEmpty(_windowsAuthConfiguration.WindowsGroupsOURoot))
            {
                foreach (var searchRoot in _windowsAuthConfiguration.WindowsGroupsOURoot.Split('|', StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
                        _AllowedWindowsGroups.AddRange(ReadADGroupsFromRoot(searchRoot));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error reading AD subtree of {searchRoot}", ex);
                    }
                }
            }
        }
        public ADProperties GetUserInfoFromAD(string userId)
        {
            var ret = new ADProperties();
            var userIdParts = userId.Split('\\', StringSplitOptions.RemoveEmptyEntries);
            var domainName = userIdParts.Length > 1 ? userIdParts.First().ToLower() : string.Empty;
            var userName = userIdParts.Last().ToLower();

            string ADSPath = $"LDAP://{domainName}";
            DirectoryEntry entry = new DirectoryEntry(ADSPath);
            if (!string.IsNullOrEmpty(_windowsAuthConfiguration.DomainUserName))
            {
                entry.Username = _windowsAuthConfiguration.DomainUserName;
                entry.Password = _windowsAuthConfiguration.DomainUserPassword;
            }
            DirectorySearcher searcher = new DirectorySearcher(entry)
            {
                Filter = $"(&(ObjectClass=person)(sAMAccountName={userName}))"
            };

            var result = searcher.FindOne();
            if (result != null)
            {
                ret.DisplayName = ReadADProperty(result.Properties, "displayName");
                ret.GivenName = ReadADProperty(result.Properties, "givenName");
                ret.FamilyName = ReadADProperty(result.Properties, "sn");
                ret.Email = ReadADProperty(result.Properties, "mail");
                ret.PhoneNumber = ReadADProperty(result.Properties, "telephoneNumber");
                ret.WebSite = ReadADProperty(result.Properties, "wWWHomePage");
                ret.Country = ReadADProperty(result.Properties, "co", "c");
                ret.StreetAddress = ReadADProperty(result.Properties, "street");

                var photoBytes = result.Properties["thumbnailPhoto"]?[0] as byte[];

                ret.Photo = $"data:image/png;base64,{Convert.ToBase64String(photoBytes)}";

                if (_windowsAuthConfiguration.IncludeWindowsGroups)
                {
                    foreach (string dn in result.Properties["memberOf"])
                    {
                        int equalsIndex = dn.IndexOf("=", 1);
                        int commaIndex = dn.IndexOf(",", 1);
                        if (equalsIndex >= 0)
                        {
                            if (commaIndex >= 0)
                                ret.Groups.Add(dn.Substring(equalsIndex + 1, commaIndex - equalsIndex - 1));
                            else
                                ret.Groups.Add(dn.Substring(equalsIndex + 1));
                        }
                    }

                    ret.Groups = new List<string>(FilterADGroups(ret.Groups));
                }
            }
            return ret;
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

        private IEnumerable<string> ReadADGroupsFromRoot(string searchroot)
        {
            string ADSPath = $"LDAP://{searchroot}";
            DirectoryEntry entry = new DirectoryEntry(ADSPath);
            if (!string.IsNullOrEmpty(_windowsAuthConfiguration.DomainUserName))
            {
                entry.Username = _windowsAuthConfiguration.DomainUserName;
                entry.Password = _windowsAuthConfiguration.DomainUserPassword;
            }

            DirectorySearcher searcher = new DirectorySearcher(entry)
            {
                SearchScope = SearchScope.Subtree,
                Filter = "(objectCategory=group)"
            };
            

            var sr = searcher.FindAll();
            foreach (var groupEntry in sr)
            {
                if (groupEntry is SearchResult res)
                {
                    if (res.Properties.Contains("cn") && res.Properties["cn"].Count > 0)
                    {
                        yield return ((string)res.Properties["cn"][0]).ToLower();
                    }
                }
            }
        }

        public IEnumerable<string> FilterADGroups(IEnumerable<string> adGroups)
        {
            if (_windowsAuthConfiguration.IncludeWindowsGroups)
            {
                foreach (var group in adGroups)
                {
                    if (string.IsNullOrEmpty(_windowsAuthConfiguration.WindowsGroupsPrefix) || group.ToLower().StartsWith(_windowsAuthConfiguration.WindowsGroupsPrefix))
                    {
                        if (_AllowedWindowsGroups == null || _AllowedWindowsGroups.Count == 0)
                            yield return group;
                        else
                        {
                            int indexOfDomainSeparator = group.IndexOf('\\');

                            var groupNameWithoutDomain = (indexOfDomainSeparator >= 0 ? group.Substring(indexOfDomainSeparator + 1) : group).ToLower();
                            if (_AllowedWindowsGroups.Contains(groupNameWithoutDomain))
                                yield return group;
                        }
                    }
                }
            }
        }

        private string ReadADProperty(ResultPropertyCollection properties, params string[] propertyName)
        {
            foreach (var p in propertyName)
            {
                if (properties[p] != null && properties[p].Count > 0 && properties[p][0] != null)
                    return properties[p][0].ToString();
            }
            return null;
        }        
    }
}
