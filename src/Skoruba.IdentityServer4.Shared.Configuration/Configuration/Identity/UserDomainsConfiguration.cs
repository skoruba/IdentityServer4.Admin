using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Skoruba.IdentityServer4.Shared.Configuration.Configuration.Identity
{
    public class UserDomainsConfiguration
    {
        /// <summary>
        /// Whether or not to allow the user administration to 
        /// assign a domain to the user.
        /// </summary>
        public bool EnableUserDomains { get; set; }

        /// <summary>
        /// Array of <see cref="UserDomain"/>
        /// </summary>
        public UserDomain[] UserDomains { get; set; }


        public UserDomainsConfiguration()
        {
            EnableUserDomains = false;
            UserDomains = new UserDomain[] { };
        }


        /// <summary>
        /// Check if the domain uses an Ldap Web Api
        /// </summary>
        /// <param name="domainId">Domain identifier. See <see cref="UserDomain"/> properties.</param>
        /// <returns><see cref="Boolean"/></returns>
        public bool CheckIfDomainUsesLdapWebApi(string domainId)
        {
            return UserDomains.Any(d => d.DomainId == (domainId ?? "") & d.UseLdapWebApi == true);
        }

        public void CheckConfigurationIntegrity()
        {
            if (!EnableUserDomains)
                return;

            if (UserDomains == null || UserDomains.Length == 0)
                throw new Exception($"Error in {nameof(UserDomainsConfiguration)} configuration section. {nameof(EnableUserDomains)} is enable, but no Domain profile is defined in {nameof(UserDomains)}.");

            var repeatedBlankDomainIds = UserDomains
                .Where(d => d.DomainId == string.Empty)
                .GroupBy(d => d.DomainId)
                .Where(g => g.Count() > 1)
                .Select(c => c.Key).ToArray();

            if (repeatedBlankDomainIds.Length > 0)
                throw new Exception($"Error in {nameof(UserDomainsConfiguration)} configuration section. Only one blank DomainId is allowed in {nameof(UserDomains)}. Take a look at {nameof(UserDomainsConfiguration)}.{nameof(UserDomains)} configuration section.");

            var blankTitleCount = UserDomains
                .Where(ud => ud.DomainId != string.Empty & ud.DomainTitle == string.Empty)
                .Count();

            if (blankTitleCount > 0)
                throw new Exception($"Error in {nameof(UserDomainsConfiguration)} configuration section. Only the blank {nameof(UserDomain.DomainId)} can have the blank {nameof(UserDomain.DomainTitle)}.");

            var repeatedDomainIds = UserDomains
                .GroupBy(d => d.DomainId)
                .Where(g => g.Count() > 1)
                .Select(c => c.Key).ToArray();

            if (repeatedDomainIds.Length > 0)
                throw new Exception($"Error in {nameof(UserDomainsConfiguration)} configuration section. Repeated {nameof(UserDomain.DomainId)} ({string.Join(", ", repeatedDomainIds)}) in {nameof(UserDomains)}. Take a look at {nameof(UserDomainsConfiguration)}.{nameof(UserDomains)} configuration section.");

            var repeatedDomainTitles = UserDomains
                .GroupBy(d => d.DomainTitle)
                .Where(g => g.Count() > 1)
                .Select(c => c.Key).ToArray();

            if (repeatedDomainTitles.Length > 0)
                throw new Exception($"Error in {nameof(UserDomainsConfiguration)} configuration section. Repeated {nameof(UserDomain.DomainTitle)} ({string.Join(", ", repeatedDomainTitles)}) in {nameof(UserDomains)}. Take a look at {nameof(UserDomainsConfiguration)}.{nameof(UserDomains)} configuration section.");
        }

        #region Inner classs
        /// <summary>
        /// Domain that can be assigned to a user.
        /// </summary>
        public class UserDomain
        {
            /// <summary>
            /// Identifier in appsettings file. Can be empty value.
            /// </summary>
            public string DomainId { get; set; }

            /// <summary>
            /// Descriptive title of the Domain Id
            /// </summary>
            public string DomainTitle { get; set; }

            /// <summary>
            /// If true, the users marked with the Domain Id 
            /// will authenticate in the corresponding LDAP Web Api.
            /// If false, users marked with the domain id will continue
            /// to authenticate with the regular method, based on the 
            /// data stored in the local repository.
            /// </summary>
            public bool UseLdapWebApi { get; set; }
        }
        #endregion
    }
}
