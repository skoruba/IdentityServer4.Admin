using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers.ADUtilities
{
    public class ADProperties
    {
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public string WebSite { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string StreetAddress { get; set; }

        public List<string> Groups { get; set; } = new List<string>();
    }
}
