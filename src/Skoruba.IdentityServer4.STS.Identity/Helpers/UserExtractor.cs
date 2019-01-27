using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entities.Identity;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public class UserExtractor
    {
        private readonly UserManager<UserIdentity> _userManager;
        private readonly LoginResolutionPolicy _policy;

        public UserExtractor(UserManager<UserIdentity> userManager, LoginConfiguration configuration)
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
            else if ((_policy == LoginResolutionPolicy.Email) ||
                ((_policy == LoginResolutionPolicy.EmailOrUsername) && emailVerifier.IsValid(login)))
            {
                return await _userManager.FindByEmailAsync(login);
            }

            return null;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UserExtractor() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
