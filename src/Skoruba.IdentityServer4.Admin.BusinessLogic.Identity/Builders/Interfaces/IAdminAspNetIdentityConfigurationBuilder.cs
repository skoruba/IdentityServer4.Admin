using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Builders.Interfaces
{
    public interface IAdminAspNetIdentityConfigurationBuilder<TKey> : IAdminAspNetIdentityConfigurationBuilder<TKey, IdentityUser<TKey>, IdentityUserClaim<TKey>, IdentityUserLogin<TKey>, IdentityUserToken<TKey>, IdentityRole<TKey>, IdentityRoleClaim<TKey>, IdentityUserRole<TKey>>
        where TKey : IEquatable<TKey>
    {
    }

    public interface IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TRole : IdentityRole<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
    {
        IAdminAspNetIdentityConfigurationBuilder<TKey, TNewUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole> UseUser<TNewUser>() where TNewUser : IdentityUser<TKey>;
        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TNewUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole> UseUserClaim<TNewUserClaim>() where TNewUserClaim : IdentityUserClaim<TKey>;
        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TNewUserLogin, TUserToken, TRole, TRoleClaim, TUserRole> UseUserLogin<TNewUserLogin>() where TNewUserLogin : IdentityUserLogin<TKey>;
        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TNewUserToken, TRole, TRoleClaim, TUserRole> UseUserToken<TNewUserToken>() where TNewUserToken : IdentityUserToken<TKey>;
        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TNewRole, TRoleClaim, TUserRole> UseRole<TNewRole>() where TNewRole : IdentityRole<TKey>;
        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TNewRoleClaim, TUserRole> UseRoleClaim<TNewRoleClaim>() where TNewRoleClaim : IdentityRoleClaim<TKey>;
        IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TNewUserRole> UseUserRole<TNewUserRole>() where TNewUserRole : IdentityUserRole<TKey>;
        IAdminAspNetIdentityConfigurationBuilderWithIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext> UseIdentityDbContext<TIdentityDbContext>() where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>;
        IAdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext> UsePersistedGrantDbContext<TPersistedGrantDbContext>() where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext;
        IAdminAspNetIdentityConfigurationBuilderWithDto<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole> UseDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure);
    }

    public interface IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TPersistedGrantDbContext>
        : IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>
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
        new IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TNewIdentityDbContext, TPersistedGrantDbContext> UseIdentityDbContext<TNewIdentityDbContext>() where TNewIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>;
        new IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TNewPersistedGrantDbContext> UsePersistedGrantDbContext<TNewPersistedGrantDbContext>() where TNewPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext;
        new IConfiguredAdminAspNetIdentityConfigurationBuilder<TKey> UseDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure);
    }

    public interface IAdminAspNetIdentityConfigurationBuilderWithIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext>
        : IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>
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
        new IAdminAspNetIdentityConfigurationBuilderWithIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TNewIdentityDbContext> UseIdentityDbContext<TNewIdentityDbContext>() where TNewIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>;
        new IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TPersistedGrantDbContext> UsePersistedGrantDbContext<TPersistedGrantDbContext>() where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext;
        new IAdminAspNetIdentityConfigurationBuilderWithDtoAndIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext> UseDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure);
    }

    public interface IAdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext>
        : IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>
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
        new IAdminAspNetIdentityConfigurationBuilderWithPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TNewPersistedGrantDbContext> UsePersistedGrantDbContext<TNewPersistedGrantDbContext>() where TNewPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext;
        new IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext, TPersistedGrantDbContext> UseIdentityDbContext<TIdentityDbContext>() where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>;
        new IAdminAspNetIdentityConfigurationBuilderWithDtoAndPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext> UseDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure);
    }
}