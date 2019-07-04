using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Stores;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Validators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Managers
{
    public class MultiTenantUserManager<TUser> : UserManager<TUser> where TUser : class
    {
        private readonly Dictionary<string, IUserTwoFactorTokenProvider<TUser>> _myProviders = new Dictionary<string, IUserTwoFactorTokenProvider<TUser>>();

        public MultiTenantUserManager(IUserStore<TUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TUser> passwordHasher,
            IEnumerable<IUserValidator<TUser>> userValidators,
            IEnumerable<IPasswordValidator<TUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<TUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            foreach (var providerName in Options.Tokens.ProviderMap.Keys)
            {
                var description = Options.Tokens.ProviderMap[providerName];

                var provider = (description.ProviderInstance ?? services.GetRequiredService(description.ProviderType))
                    as IUserTwoFactorTokenProvider<TUser>;
                if (provider != null)
                {
                    _myProviders.Add(providerName, provider);
                }
            }
        }

        [Obsolete]
        public override Task<TUser> FindByNameAsync(string userName)
        {
            return null;
        }

        [Obsolete]
        public override Task<TUser> FindByEmailAsync(string email)
        {
            return null;
        }

        public Task<TUser> FindByEmailAsync(string tenantCode, string email)
        {
            return (Store as IMultiTenantUserStore<TUser>).FindByEmailAsync(tenantCode, email);
        }

        public Task<TUser> FindByNameAsync(string tenantCode, string userName)
        {
            return (Store as IMultiTenantUserStore<TUser>).FindByNameAsync(tenantCode, userName);
        }
    }

    public static class UserManagerExtensions
    {
        public static Task<TUser> FindByEmailAsync<TUser>(this UserManager<TUser> userManager, string tenantCode, string email) where TUser : class
        {
            return (userManager as MultiTenantUserManager<TUser>).FindByEmailAsync(tenantCode, email);
        }

        public static Task<TUser> FindByNameAsync<TUser>(this UserManager<TUser> userManager, string tenantCode, string userName) where TUser : class
        {
            return (userManager as MultiTenantUserManager<TUser>).FindByNameAsync(tenantCode, userName);
        }

        public static bool IsMultiTenant<TUser>(this UserManager<TUser> userManager) where TUser : class
        {
            var a = userManager as MultiTenantUserManager<TUser>;

            return (userManager as MultiTenantUserManager<TUser>) != null;
        }
    }
}