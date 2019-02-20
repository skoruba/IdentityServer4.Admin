using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using System.ComponentModel.DataAnnotations;
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
            // edge case when someone allowed @ in usernames
            // use username then
            if (identityOptions.Value.User.AllowedUserNameCharacters.Contains('@') && (configuration.ResolutionPolicy == LoginResolutionPolicy.EmailOrUsername))
            {
                _logger.LogWarning($"Selected login resolution policy is ambigous given '@' allowed in username characters. Defaulting to username policy");
                _policy = LoginResolutionPolicy.Username;
            } else
            {
                _policy = configuration.ResolutionPolicy;
            }
        }

        public async Task<TUser> GetUserAsync(string login)
        {
            var emailVerifier = new EmailAddressAttribute();

            if ((_policy == LoginResolutionPolicy.Username) || 
                ((_policy == LoginResolutionPolicy.EmailOrUsername) && !emailVerifier.IsValid(login)))
            {
                return await _userManager.FindByNameAsync(login);
            }
            else if (_policy == LoginResolutionPolicy.Email)
            {
                return await _userManager.FindByEmailAsync(login);
            }
            else if ((_policy == LoginResolutionPolicy.EmailOrUsername) && emailVerifier.IsValid(login))
            {
                var user = await _userManager.FindByEmailAsync(login);

                if (user != default(TUser))
                {
                    return user;
                }

                user = await _userManager.FindByNameAsync(login);

                if (user != default(TUser))
                {
                    return user;
                }
            }

            return null;
        }
    }
}
