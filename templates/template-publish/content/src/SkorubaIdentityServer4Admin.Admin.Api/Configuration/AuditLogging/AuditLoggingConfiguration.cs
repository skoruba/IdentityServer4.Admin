namespace SkorubaIdentityServer4Admin.Admin.Api.Configuration
{
    public class AuditLoggingConfiguration
    {
        public string Source { get; set; }

        public string SubjectIdentifierClaim { get; set; }

        public string SubjectNameClaim { get; set; }

        public string ClientIdClaim { get; set; }
    }
}






