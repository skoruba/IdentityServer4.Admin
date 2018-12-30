namespace SkorubaIdentityServer4Admin.STS.Identity.Configuration
{
	public class CertificateConfiguration
	{
		public bool UseTemporaryKey { get; set; }

		public string SigningCertificateThumbprint { get; set; }

		public string ValidationCertificateThumbprint { get; set; }
	}
}
