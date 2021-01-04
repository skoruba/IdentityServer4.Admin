using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.MobileAuth.Services
{
    public interface ISmsService
    {
        Task<bool> SendAsync(string phoneNumber, string body);
    }
}