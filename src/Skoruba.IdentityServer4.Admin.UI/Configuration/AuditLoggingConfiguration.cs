namespace Skoruba.IdentityServer4.Admin.Configuration
{
    public class AuditLoggingConfiguration
    {
        public string Source { get; set; }

        public string SubjectIdentifierClaim { get; set; }

        public string SubjectNameClaim { get; set; }

        public bool IncludeFormVariables { get; set; }
    }
}