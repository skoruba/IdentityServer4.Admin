using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public class UserResolver<TUser> where TUser : class
    {
        private readonly UserManager<TUser> _userManager;
        private readonly ILogger<UserResolver<TUser>> _logger;
        private readonly LoginResolutionPolicy _policy;

        public UserResolver(UserManager<TUser> userManager, LoginConfiguration configuration, IOptions<IdentityOptions> identityOptions, ILogger<UserResolver<TUser>> logger)
        {
            _userManager = userManager;
            _logger = logger;
            _policy = configuration.ResolutionPolicy;
        }

        public async Task<TUser> GetUserAsync(string login)
        {
            if (_policy == LoginResolutionPolicy.Username)
            {
                return await _userManager.FindByNameAsync(login);
            }
            else if (_policy == LoginResolutionPolicy.Email)
            {
                return await _userManager.FindByEmailAsync(login);
            }

            return null;
        }
    }
}
