using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Managers;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public class UserResolver<TUser> where TUser : class
    {
        private readonly UserManager<TUser> _userManager;
        private readonly LoginResolutionPolicy _policy;

        public UserResolver(UserManager<TUser> userManager, LoginConfiguration configuration)
        {
            _userManager = userManager;
            _policy = configuration.ResolutionPolicy;
        }

        public async Task<TUser> GetUserAsync(string login, string tenant = null)
        {
            if (_userManager.IsMultiTenant())
            {
                switch (_policy)
                {
                    case LoginResolutionPolicy.Username:
                        return await _userManager.FindByNameAsync(login, tenant);

                    case LoginResolutionPolicy.Email:
                        return await _userManager.FindByEmailAsync(login, tenant);

                    default:
                        return null;
                }
            }
            else
            {
                switch (_policy)
                {
                    case LoginResolutionPolicy.Username:
                        return await _userManager.FindByNameAsync(login);

                    case LoginResolutionPolicy.Email:
                        return await _userManager.FindByEmailAsync(login);

                    default:
                        return null;
                }
            }
        }
    }
}