namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class CertificateConfiguration
    {
        public bool UseTemporaryKey { get; set; }

        public bool UseSigningCertificateThumbprint { get; set; }

        public bool UseSigningCertificateFile { get; set; }

        public string SigningCertificateThumbprint { get; set; }

        public string SigningCertificateFilePath { get; set; }

        public string SigningCertificateFilePassword { get; set; }

        public bool UseValidationCertificateThumbprint { get; set; }

        public bool UseValidationCertificateFile { get; set; }

        public string ValidationCertificateThumbprint { get; set; }

        public string ValidationCertificateFilePath { get; set; }

        public string ValidationCertificateFilePassword { get; set; }
    }
}
