using System;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Builders
{
    public struct AdminAspNetIdentityConfiguration<TKey> where TKey : IEquatable<TKey>
    {
        public struct Dto
        {
            public Type Role { get; internal set; }
            public Type Roles { get; internal set; }
            public Type RoleClaim { get; internal set; }
            public Type RoleClaims { get; internal set; }
            public Type UserRoles { get; internal set; }
            public Type User { get; internal set; }
            public Type Users { get; internal set; }
            public Type UserClaim { get; internal set; }
            public Type UserClaims { get; internal set; }
            public Type UserProvider { get; internal set; }
            public Type UserProviders { get; internal set; }
            public Type UserChangePassword { get; internal set; }
        }

        public Type UserType { get; internal set; }
        public Type UserClaimType { get; internal set; }
        public Type UserLoginType { get; internal set; }
        public Type UserTokenType { get; internal set; }
        public Type RoleType { get; internal set; }
        public Type RoleClaimType { get; internal set; }
        public Type UserRoleType { get; internal set; }
        public Type IdentityDbContextType { get; internal set; }
        public Type PersistedGrantDbContextType { get; internal set; }
        public Dto DtoTypes { get; internal set; }
    }
}