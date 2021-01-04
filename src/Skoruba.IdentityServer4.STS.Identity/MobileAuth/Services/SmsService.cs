using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.MobileAuth.Services
{
    public class SmsService : ISmsService
    {
        public Task<bool> SendAsync(string phoneNumber, string body)
        {
            return Task.FromResult(true);
        }
    }
}