namespace SkorubaIdentityServer4Admin.STS.Identity.Configuration
{
    public class CertificateConfiguration
    {
        public bool UseTemporarySigningKeyForDevelopment { get; set; }

        public string CertificateStoreLocation { get; set; }
        public bool CertificateValidOnly { get; set; }

        public bool UseSigningCertificateThumbprint { get; set; }

        public string SigningCertificateThumbprint { get; set; }

        public bool UseSigningCertificatePfxFile { get; set; }        

        public string SigningCertificatePfxFilePath { get; set; }

        public string SigningCertificatePfxFilePassword { get; set; }

        public bool UseValidationCertificateThumbprint { get; set; }        

        public string ValidationCertificateThumbprint { get; set; }

        public bool UseValidationCertificatePfxFile { get; set; }

        public string ValidationCertificatePfxFilePath { get; set; }

        public string ValidationCertificatePfxFilePassword { get; set; }
    }
}






