using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public class UserResolver
    {
        private readonly UserManager<UserIdentity> _userManager;
        private readonly LoginResolutionPolicy _policy;

        public UserResolver(UserManager<UserIdentity> userManager, LoginConfiguration configuration)
        {
            _userManager = userManager;
            _policy = configuration.ResolutionPolicy;
        }

        public async Task<UserIdentity> GetUserAsync(string login)
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

                if (user != default(UserIdentity))
                {
                    return user;
                }

                user = await _userManager.FindByNameAsync(login);

                if (user != default(UserIdentity))
                {
                    return user;
                }
            }

            return null;
        }
    }
}
