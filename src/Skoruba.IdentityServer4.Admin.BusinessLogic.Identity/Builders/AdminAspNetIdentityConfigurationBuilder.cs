using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Builders.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Builders
{
    internal class AdminAspNetIdentityConfigurationBuilder<TKey> : AdminAspNetIdentityConfigurationBuilder<TKey, IdentityUser<TKey>, IdentityUserClaim<TKey>, IdentityUserLogin<TKey>, IdentityUserToken<TKey>, IdentityRole<TKey>, IdentityRoleClaim<TKey>, IdentityUserRole<TKey>>, IAdminAspNetIdentityConfigurationBuilder<TKey>
        where TKey : IEquatable<TKey>
    {
        private static readonly AdminAspNetIdentityConfiguration<TKey>.Dto DefaultDto = new AdminAspNetIdentityConfiguration<TKey>.Dto
        {
            Role = typeof(RoleDto<TKey>),
            Roles = typeof(RolesDto<RoleDto<TKey>, TKey>),
            RoleClaim = typeof(RoleClaimDto<TKey>),
            RoleClaims = typeof(RoleClaimsDto<TKey>),
            UserRoles = typeof(UserRolesDto<RoleDto<TKey>, TKey>),
            User = typeof(UserDto<TKey>),
            Users = typeof(UsersDto<UserDto<TKey>, TKey>),
            UserClaim = typeof(UserClaimDto<TKey>),
            UserClaims = typeof(UserClaimsDto<TKey>),
            UserProvider = typeof(UserProviderDto<TKey>),
            UserProviders = typeof(UserProvidersDto<TKey>),
            UserChangePassword = typeof(UserChangePasswordDto<TKey>)
        };

        public AdminAspNetIdentityConfigurationBuilder() : base(DefaultDto)
        {
        }
    }

    internal class AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>
        : IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>,
          IAdminAspNetIdentityConfigurationBuilderWithDto<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TRole : IdentityRole<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
    {
        protected AdminAspNetIdentityConfiguration<TKey>.Dto dto;

        public AdminAspNetIdentityConfigurationBuilder(AdminAspNetIdentityConfiguration<TKey>.Dto dto) => this.dto = dto;

        protected AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole> updateDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure = null)
        {
            var dtoBuilder = new AdminAspNetIdentityDtoConfigurationBuilder<TKey>();
            configure?.Invoke(dtoBuilder);
            dto = dtoBuilder.Build();
            return this;
        }

        IAdminAspNetIdentityConfigurationBuilderWithDto<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole> IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>.UseDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure) => updateDto(configure);

        IAdminAspNetIdentityConfigurationBuilderWithDto<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole> IAdminAspNetIdentityConfigurationBuilderWithDto<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>.UseDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure) => updateDto(configure);

        IAdminAspNetIdentityConfigurationBuilderWithIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext> IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>.UseIdentityDbContext<TIdentityDbContext>() =>
            new AdminAspNetIdentityConfigurationBuilderWithIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext>(dto);

        IAdminAspNetIdentityConfigurationBuilderWithDtoAndIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext> IAdminAspNetIdentityConfigurationBuilderWithDto<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>.UseIdentityDbContext<TIdentityDbContext>() =>
            new AdminAspNetIdentityConfigurationBuilderWithIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext>(dto);

        IAdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext> IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>.UsePersistedGrantDbContext<TPersistedGrantDbContext>() =>
            new AdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext>(dto);

        IAdminAspNetIdentityConfigurationBuilderWithDtoAndPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext> IAdminAspNetIdentityConfigurationBuilderWithDto<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>.UsePersistedGrantDbContext<TPersistedGrantDbContext>() =>
            new AdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext>(dto);

        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TNewRole, TRoleClaim, TUserRole> IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>.UseRole<TNewRole>() =>
            new AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TNewRole, TRoleClaim, TUserRole>(dto);

        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TNewRoleClaim, TUserRole> IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>.UseRoleClaim<TNewRoleClaim>() =>
            new AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TNewRoleClaim, TUserRole>(dto);

        IAdminAspNetIdentityConfigurationBuilder<TKey, TNewUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole> IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>.UseUser<TNewUser>() =>
            new AdminAspNetIdentityConfigurationBuilder<TKey, TNewUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>(dto);

        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TNewUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole> IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>.UseUserClaim<TNewUserClaim>() =>
            new AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TNewUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>(dto);

        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TNewUserLogin, TUserToken, TRole, TRoleClaim, TUserRole> IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>.UseUserLogin<TNewUserLogin>() =>
            new AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TNewUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>(dto);

        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TNewUserRole> IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>.UseUserRole<TNewUserRole>() =>
            new AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TNewUserRole>(dto);

        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TNewUserToken, TRole, TRoleClaim, TUserRole> IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>.UseUserToken<TNewUserToken>() =>
            new AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TNewUserToken, TRole, TRoleClaim, TUserRole>(dto);
    }

    internal class AdminAspNetIdentityConfigurationBuilderWithIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext>
        : AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>,
          IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>,
          IAdminAspNetIdentityConfigurationBuilderWithIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext>,
          IAdminAspNetIdentityConfigurationBuilderWithDto<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>,
          IAdminAspNetIdentityConfigurationBuilderWithDtoAndIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TRole : IdentityRole<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
    {
        public AdminAspNetIdentityConfigurationBuilderWithIdentityDbContext(AdminAspNetIdentityConfiguration<TKey>.Dto dto) : base(dto)
        {
        }

        private new AdminAspNetIdentityConfigurationBuilderWithIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext> updateDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure = null)
        {
            base.updateDto();
            return this;
        }

        IAdminAspNetIdentityConfigurationBuilderWithDtoAndIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext> IAdminAspNetIdentityConfigurationBuilderWithIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext>.UseDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure) => updateDto(configure);

        IAdminAspNetIdentityConfigurationBuilderWithDtoAndIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext> IAdminAspNetIdentityConfigurationBuilderWithDtoAndIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext>.UseDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure) => updateDto(configure);

        IAdminAspNetIdentityConfigurationBuilderWithIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TNewIdentityDbContext> IAdminAspNetIdentityConfigurationBuilderWithIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext>.UseIdentityDbContext<TNewIdentityDbContext>() =>
            new AdminAspNetIdentityConfigurationBuilderWithIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TNewIdentityDbContext>(dto);

        IAdminAspNetIdentityConfigurationBuilderWithDtoAndIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TNewIdentityDbContext> IAdminAspNetIdentityConfigurationBuilderWithDtoAndIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext>.UseIdentityDbContext<TNewIdentityDbContext>() =>
            new AdminAspNetIdentityConfigurationBuilderWithIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TNewIdentityDbContext>(dto);

        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TPersistedGrantDbContext> IAdminAspNetIdentityConfigurationBuilderWithIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext>.UsePersistedGrantDbContext<TPersistedGrantDbContext>() =>
            new AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TPersistedGrantDbContext>(dto);

        IConfiguredAdminAspNetIdentityConfigurationBuilder<TKey> IAdminAspNetIdentityConfigurationBuilderWithDtoAndIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext>.UsePersistedGrantDbContext<TPersistedGrantDbContext>() =>
            new AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TPersistedGrantDbContext>(dto);
    }

    internal class AdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext>
        : AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>,
          IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>,
          IAdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext>,
          IAdminAspNetIdentityConfigurationBuilderWithDto<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>,
          IAdminAspNetIdentityConfigurationBuilderWithDtoAndPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TRole : IdentityRole<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
    {
        public AdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext(AdminAspNetIdentityConfiguration<TKey>.Dto dto) : base(dto)
        {
        }

        private new AdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext> updateDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure = null)
        {
            base.updateDto();
            return this;
        }

        IAdminAspNetIdentityConfigurationBuilderWithDtoAndPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext> IAdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext>.UseDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure) => updateDto(configure);

        IAdminAspNetIdentityConfigurationBuilderWithDtoAndPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext> IAdminAspNetIdentityConfigurationBuilderWithDtoAndPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext>.UseDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure) => updateDto(configure);

        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TPersistedGrantDbContext> IAdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext>.UseIdentityDbContext<TIdentityDbContext>() =>
            new AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TPersistedGrantDbContext>(dto);

        IConfiguredAdminAspNetIdentityConfigurationBuilder<TKey> IAdminAspNetIdentityConfigurationBuilderWithDtoAndPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext>.UseIdentityDbContext<TIdentityDbContext>() =>
            new AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TPersistedGrantDbContext>(dto);

        IAdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TNewPersistedGrantDbContext> IAdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext>.UsePersistedGrantDbContext<TNewPersistedGrantDbContext>() =>
            new AdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TNewPersistedGrantDbContext>(dto);

        IAdminAspNetIdentityConfigurationBuilderWithDtoAndPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TNewPersistedGrantDbContext> IAdminAspNetIdentityConfigurationBuilderWithDtoAndPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext>.UsePersistedGrantDbContext<TNewPersistedGrantDbContext>() =>
            new AdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TNewPersistedGrantDbContext>(dto);
    }

    internal class AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TPersistedGrantDbContext>
        : AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>,
          IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>,
          IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TPersistedGrantDbContext>,
          IAdminAspNetIdentityConfigurationBuilderWithDto<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>,
          IConfiguredAdminAspNetIdentityConfigurationBuilder<TKey>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TRole : IdentityRole<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
    {
        public AdminAspNetIdentityConfigurationBuilder(AdminAspNetIdentityConfiguration<TKey>.Dto dto) : base(dto)
        {
        }

        private new AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TPersistedGrantDbContext> updateDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure = null)
        {
            base.updateDto();
            return this;
        }

        AdminAspNetIdentityConfiguration<TKey> IConfiguredAdminAspNetIdentityConfigurationBuilder<TKey>.Build() => new AdminAspNetIdentityConfiguration<TKey>
        {
            UserType = typeof(TUser),
            UserClaimType = typeof(TUserClaim),
            UserLoginType = typeof(TUserLogin),
            UserTokenType = typeof(TUserToken),
            RoleType = typeof(TRole),
            RoleClaimType = typeof(TRoleClaim),
            UserRoleType = typeof(TUserRole),
            IdentityDbContextType = typeof(TIdentityDbContext),
            PersistedGrantDbContextType = typeof(TPersistedGrantDbContext),
            DtoTypes = dto
        };

        IConfiguredAdminAspNetIdentityConfigurationBuilder<TKey> IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TPersistedGrantDbContext>.UseDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure) => updateDto(configure);

        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TNewIdentityDbContext, TPersistedGrantDbContext> IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TPersistedGrantDbContext>.UseIdentityDbContext<TNewIdentityDbContext>() =>
            new AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TNewIdentityDbContext, TPersistedGrantDbContext>(dto);

        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TNewPersistedGrantDbContext> IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TPersistedGrantDbContext>.UsePersistedGrantDbContext<TNewPersistedGrantDbContext>() =>
            new AdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TNewPersistedGrantDbContext>(dto);
    }
}