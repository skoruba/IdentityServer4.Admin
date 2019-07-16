using Skoruba.IdentityServer4.STS.Identity.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers.ADUtilities
{
    public class ADUtilities : IADUtilities
    {
        protected readonly LoginConfiguration _loginConfiguration;

        protected readonly List<string> _AllowedWindowsGroups = new List<string>();
        
        public ADUtilities(LoginConfiguration loginConfiguration)
        {
            _loginConfiguration = loginConfiguration;

            if (!string.IsNullOrEmpty(_loginConfiguration.WindowsGroupsOURoot))
            {
                foreach (var searchRoot in _loginConfiguration.WindowsGroupsOURoot.Split('|', StringSplitOptions.RemoveEmptyEntries))
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
            var searcher = new DirectorySearcher($"LDAP://{domainName}")
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

                if (_loginConfiguration.IncludeWindowsGroups)
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

        private IEnumerable<string> ReadADGroupsFromRoot(string searchroot)
        {
            DirectorySearcher ds = new DirectorySearcher();
            var adsearchRoot = new DirectoryEntry(string.Format("LDAP://{0}", searchroot));
            ds.SearchRoot = adsearchRoot;
            ds.SearchScope = SearchScope.Subtree;
            ds.Filter = "(objectCategory=group)";
            var sr = ds.FindAll();
            foreach (var entry in sr)
            {
                if (entry is SearchResult res)
                {
                    if (res.Properties.Contains("cn") && res.Properties["cn"].Count > 0)
                    {
                        yield return ((string)res.Properties["cn"][0]).ToLower();
                    }
                }
            }
        }

        private IEnumerable<string> FilterADGroups(IEnumerable<string> adGroups)
        {
            if (_loginConfiguration.IncludeWindowsGroups)
            {
                foreach (var group in adGroups)
                {
                    if (string.IsNullOrEmpty(_loginConfiguration.WindowsGroupsPrefix) || group.ToLower().StartsWith(_loginConfiguration.WindowsGroupsPrefix))
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
