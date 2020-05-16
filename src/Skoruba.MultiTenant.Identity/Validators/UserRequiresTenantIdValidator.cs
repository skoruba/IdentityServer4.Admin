﻿using Microsoft.AspNetCore.Identity;
using Skoruba.MultiTenant.Abstractions;
using System.Threading.Tasks;

namespace Skoruba.MultiTenant.Identity.Validators
{
    /// <summary>
    /// Verifies the user has a tenant id
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class UserRequiresTenantIdValidator<TUser> : IUserValidator<TUser>
        where TUser : class
    {
        private readonly ISkorubaTenantContext _skorubaTenantContext;

        public UserRequiresTenantIdValidator(ISkorubaTenantContext skorubaTenantContext)
        {
            _skorubaTenantContext = skorubaTenantContext;
        }

        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            if (_skorubaTenantContext.TenantResolutionRequired && !_skorubaTenantContext.TenantResolved)
            {
                throw MultiTenantException.MissingTenant;
            }
            if (string.IsNullOrEmpty(((IHaveTenantId)user).TenantId))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "Tenant_Required", Description = "A tenant id is required." }));
            }
            if (((IHaveTenantId)user).TenantId != _skorubaTenantContext.Tenant.Id)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "Tenant_Invalid", Description = "The tenant id must be the same as the current user's tenant id." }));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
