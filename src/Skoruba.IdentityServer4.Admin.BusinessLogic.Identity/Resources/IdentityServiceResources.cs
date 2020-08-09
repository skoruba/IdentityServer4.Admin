using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Helpers;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources
{
    public class IdentityServiceResources : IIdentityServiceResources
    {
        public virtual ResourceMessage UserUpdateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserUpdateFailed),
                Description = IdentityServiceResource.UserUpdateFailed
            };
        }

        public virtual ResourceMessage UserRoleDeleteFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserRoleDeleteFailed),
                Description = IdentityServiceResource.UserRoleDeleteFailed
            };
        }

        public virtual ResourceMessage UserRoleCreateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserRoleCreateFailed),
                Description = IdentityServiceResource.UserRoleCreateFailed
            };
        }
    
        public virtual ResourceMessage UserProviderDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserProviderDoesNotExist),
                Description = IdentityServiceResource.UserProviderDoesNotExist
            };
        }

        public virtual ResourceMessage UserProviderDeleteFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserProviderDeleteFailed),
                Description = IdentityServiceResource.UserProviderDeleteFailed
            };
        }

        public virtual ResourceMessage UserChangePasswordFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserChangePasswordFailed),
                Description = IdentityServiceResource.UserChangePasswordFailed
            };
        }

        public virtual ResourceMessage UserDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserDoesNotExist),
                Description = IdentityServiceResource.UserDoesNotExist
            };
        }

        public virtual ResourceMessage UserDeleteFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserDeleteFailed),
                Description = IdentityServiceResource.UserDeleteFailed
            };
        }

        public virtual ResourceMessage UserCreateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserCreateFailed),
                Description = IdentityServiceResource.UserCreateFailed
            };
        }

        public virtual ResourceMessage UserClaimsDeleteFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserClaimsDeleteFailed),
                Description = IdentityServiceResource.UserClaimsDeleteFailed
            };
        }

        public virtual ResourceMessage UserClaimsCreateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserClaimsCreateFailed),
                Description = IdentityServiceResource.UserClaimsCreateFailed
            };
        }

        public virtual ResourceMessage UserClaimsUpdateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserClaimsCreateFailed),
                Description = IdentityServiceResource.UserClaimsUpdateFailed
            };
        }

        public virtual ResourceMessage UserClaimDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserClaimDoesNotExist),
                Description = IdentityServiceResource.UserClaimDoesNotExist
            };
        }

        public virtual ResourceMessage RoleUpdateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleUpdateFailed),
                Description = IdentityServiceResource.RoleUpdateFailed
            };
        }

        public virtual ResourceMessage RoleDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleDoesNotExist),
                Description = IdentityServiceResource.RoleDoesNotExist
            };
        }

        public virtual ResourceMessage RoleDeleteFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleDeleteFailed),
                Description = IdentityServiceResource.RoleDeleteFailed
            };
        }

        public virtual ResourceMessage RoleCreateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleCreateFailed),
                Description = IdentityServiceResource.RoleCreateFailed
            };
        }

        public virtual ResourceMessage RoleClaimsDeleteFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleClaimsDeleteFailed),
                Description = IdentityServiceResource.RoleClaimsDeleteFailed
            };
        }

        public virtual ResourceMessage RoleClaimsCreateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleClaimsCreateFailed),
                Description = IdentityServiceResource.RoleClaimsCreateFailed
            };
        }

        public virtual ResourceMessage RoleClaimsUpdateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleClaimsCreateFailed),
                Description = IdentityServiceResource.RoleClaimsUpdateFailed
            };
        }

        public virtual ResourceMessage RoleClaimDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleClaimDoesNotExist),
                Description = IdentityServiceResource.RoleClaimDoesNotExist
            };
        }

        public virtual ResourceMessage IdentityErrorKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(IdentityErrorKey),
                Description = IdentityServiceResource.IdentityErrorKey
            };
        }
    }
}
