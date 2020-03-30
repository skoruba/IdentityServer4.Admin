using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers.ADServices
{
    public class ADUserInfoExtractor
    {
        private readonly ILogger<ADUserInfoExtractor> _logger;
        private readonly IOptions<WindowsAuthConfiguration> _windowsAuthConfiguration;

        public ADUserInfoExtractor(
            ILogger<ADUserInfoExtractor> logger,
            IOptions<WindowsAuthConfiguration> windowsAuthConfiguration
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _windowsAuthConfiguration = windowsAuthConfiguration ?? throw new ArgumentNullException(nameof(windowsAuthConfiguration));
        }

        public bool GetProviderKeyFQDN(string userIdentity, out string domainFQDN, out string userName)
        {
            domainFQDN = null;
            userName = null;
            var split = userIdentity.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Count() != 2)
                return false;
            domainFQDN = split[0];
            if (domainFQDN != null)
            {
                using (var domainContext = new PrincipalContext(ContextType.Domain, domainFQDN))
                {
                    DirectoryContext context = new DirectoryContext(DirectoryContextType.Domain, domainFQDN);
                    Domain domain = Domain.GetDomain(context);
                    domainFQDN = domain.Name;
                }
            }
            userName = split[1];
            return true;
        }

        public ADUserInfo GetADUserInfo(string userIdentity)
        {
            if(GetProviderKeyFQDN(userIdentity, out var domainFQDN, out var username))
                return GetADUserInfo(username, domainFQDN);
            return null;
        }

        public ADUserInfo GetADUserInfo(string username, string domainFQDN)
        { 
            var domainConfig = _windowsAuthConfiguration.Value?.Domains?.Where(p => p.DomainFullQualifiedName == domainFQDN).FirstOrDefault();
            PrincipalContext ctx = null;
            if (domainConfig != null && !string.IsNullOrWhiteSpace(domainConfig.DomainUserName))
                ctx = new PrincipalContext(ContextType.Domain, domainConfig.DomainFullQualifiedName, domainConfig.DomainUserName, domainConfig.DomainUserPassword);
            else
                ctx = new PrincipalContext(ContextType.Domain, domainFQDN);
            using (ctx)
            {
                UserPrincipal up = new UserPrincipal(ctx);
                PrincipalSearcher ps = new PrincipalSearcher(up);
                ps.QueryFilter = new UserPrincipal(ctx) { SamAccountName = username };
                var principalFound = ps.FindOne();
                var userPrincipalFound = principalFound as UserPrincipal;
                if (userPrincipalFound != null)
                {
                    var adProperties = GetADProperties(userPrincipalFound, domainConfig);
                    return adProperties;
                }
            }

            return null;
        }

        public IEnumerable<ADUserInfo> GetAllUsersFromAD(ActiveDirectoryDomainConfiguration adDomainConfiguration)
        {
            List<ADUserInfo> results = new List<ADUserInfo>();
            if (adDomainConfiguration != null && !string.IsNullOrWhiteSpace(adDomainConfiguration.DomainFullQualifiedName))
            {
                try
                {
                    PrincipalContext ctx = null;
                    if (!string.IsNullOrWhiteSpace(adDomainConfiguration.DomainUserName))
                        ctx = new PrincipalContext(ContextType.Domain, adDomainConfiguration.DomainFullQualifiedName, adDomainConfiguration.DomainUserName, adDomainConfiguration.DomainUserPassword);
                    else
                        ctx = new PrincipalContext(ContextType.Domain, adDomainConfiguration.DomainFullQualifiedName);
                    using (ctx)
                    {
                        UserPrincipal u = new UserPrincipal(ctx);

                        PrincipalSearcher ps = new PrincipalSearcher(u);
                        var principals = ps.FindAll();
                        foreach (var principal in principals)
                        {
                            var userPrincipal = principal as UserPrincipal;
                            if (userPrincipal != null)
                            {
                                var adProperties = GetADProperties(userPrincipal, adDomainConfiguration);
                                if (adProperties != null)
                                {
                                    results.Add(adProperties);
                                }
                            }
                        }
                    }
                }
                catch (PrincipalServerDownException ex)
                {
                    _logger.LogError($"Cannot retrieve users for AD domain {adDomainConfiguration}: {ex.Message}", ex);
                }
            }
            return results;
        }

        private ADUserInfo GetADProperties(UserPrincipal userPrincipal, ActiveDirectoryDomainConfiguration adDomainConfiguration)
        {
            if (string.IsNullOrWhiteSpace(userPrincipal.SamAccountName) || string.IsNullOrWhiteSpace(userPrincipal.EmailAddress) || string.IsNullOrWhiteSpace(userPrincipal.DisplayName))
            {
                _logger.LogDebug($"User SID {userPrincipal.Sid?.ToString()} has some missing mandatory properties: samAccountName={userPrincipal.SamAccountName}, email={userPrincipal.EmailAddress}, displayName={userPrincipal.DisplayName}");
                return null;
            }

            var adProperties = new ADUserInfo();
            var username = userPrincipal.SamAccountName.Split('\\', StringSplitOptions.RemoveEmptyEntries).Last().ToLower();
            var domainName = userPrincipal.Context.Name;
            adProperties.Username = username;
            adProperties.DomainFQDN = domainName;
            adProperties.ObjectGuid = userPrincipal.Guid?.ToString();
            adProperties.DisplayName = userPrincipal.DisplayName;
            adProperties.GivenName = userPrincipal.GivenName;
            adProperties.FamilyName = userPrincipal.Surname;
            adProperties.Email = userPrincipal.EmailAddress;
            if (ReadADProperty(userPrincipal, new string[] { "telephoneNumber" }, out var telephoneNumber))
                adProperties.PhoneNumber = (string)telephoneNumber;
            if (ReadADProperty(userPrincipal, new string[] { "wWWHomePage" }, out var webSite))
                adProperties.WebSite = (string)webSite;
            if (ReadADProperty(userPrincipal, new string[] { "co", "c" }, out var country))
                adProperties.Country = (string)country;
            if (ReadADProperty(userPrincipal, new string[] { "street" }, out var street))
                adProperties.StreetAddress = (string)street;

            if (adDomainConfiguration != null)
            {
                if (adDomainConfiguration.GetPhotoThumbnailFromAD)
                {
                    if (ReadADProperty(userPrincipal, new string[] { "thumbnailPhoto" }, out var thumbnailPhoto))
                    {
                        var thumbnailPhotoObjectCast = (thumbnailPhoto as object[]);
                        if (thumbnailPhotoObjectCast != null && thumbnailPhotoObjectCast.Count() > 0)
                        {
                            var photoBytes = thumbnailPhotoObjectCast[0] as byte[];
                            if (photoBytes.Length > 0)//How to obtain the real image format
                                adProperties.Photo = $"data:image/png;base64,{Convert.ToBase64String(photoBytes)}";
                        }
                    }
                }

                if (adDomainConfiguration.IncludeWindowsGroups)
                {
                    if (ReadADProperty(userPrincipal, new string[] { "memberOf" }, out var groups))
                    {
                        var groupsListOfStrings = groups as IEnumerable<object>;
                        if (groupsListOfStrings == null)//Fallback to one string
                        {
                            if ((groups as string) != null)
                                groupsListOfStrings = new object[] { groups };
                        }
                        if (groupsListOfStrings != null)
                        {
                            foreach (var group in groupsListOfStrings.OfType<string>())
                            {
                                int equalsIndex = group.IndexOf("=", 1);
                                int commaIndex = group.IndexOf(",", 1);
                                if (equalsIndex >= 0)
                                {
                                    if (commaIndex >= 0)
                                        adProperties.Groups.Add(group.Substring(equalsIndex + 1, commaIndex - equalsIndex - 1));
                                    else
                                        adProperties.Groups.Add(group.Substring(equalsIndex + 1));
                                }
                            }
                        }
                    }
                }
            }
            return adProperties;
        }

        private bool ReadADProperty(UserPrincipal userPrincipal, IEnumerable<string> possiblePropertyNames, out object propertyValue)
        {
            propertyValue = default;
            if (userPrincipal.GetUnderlyingObjectType() == typeof(DirectoryEntry))
            {
                var directoryEntry = (DirectoryEntry)userPrincipal.GetUnderlyingObject();
                foreach (var propertyName in possiblePropertyNames)
                {
                    if (directoryEntry.Properties.Contains(propertyName))
                    {
                        propertyValue = directoryEntry.Properties[propertyName].Value;
                        return propertyValue != null;
                    }
                }
            }
            return false;
        }

    }
}
