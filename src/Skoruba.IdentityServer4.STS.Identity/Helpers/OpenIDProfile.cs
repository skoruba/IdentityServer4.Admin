namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public class OpenIdProfile
    {
        public string FirstName { get; internal set; }
        public string Name { get; internal set; }
        public string MiddleName { get; internal set; }
        //public EGenders Gender { get; internal set; }
        public string UserType { get; internal set; }
        public bool SignNotifyActive { get; internal set; }
        public bool SignPushNotifyActive { get; internal set; }
        public string RegionAddrRef { get; internal set; }
        public bool UserFromEsia { get; internal set; }
        public bool SignAutoLocationActive { get; internal set; }
        public string AddressFias { get; internal set; }
        public string LoginNameIp { get; internal set; }
        public string LoginNameUl { get; internal set; }
        public string NameOrg { get; internal set; }
        public string OgrnIp { get; internal set; }
        public string OgrnUl { get; internal set; }
        public string Opf { get; internal set; }
        public bool ShowSvetAttributes { get; internal set; }
        public bool ShowExtendedAttributes { get; internal set; }

        public string Website { get; internal set; }
        public string Profile { get; internal set; }
        public string StreetAddress { get; internal set; }
        public string Locality { get; internal set; }
        public string Region { get; internal set; }
        public string PostalCode { get; internal set; }
        public string Country { get; internal set; }
    }
}
