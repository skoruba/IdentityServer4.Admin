using System.Security.Cryptography.X509Certificates;

namespace IdentityServer4.STS.Identity.Helpers.Certificate
{
    public interface ICertificateService
    {
        X509Certificate2 GetCertificateFromKeyVault(string vaultCertificateName);
    }
}