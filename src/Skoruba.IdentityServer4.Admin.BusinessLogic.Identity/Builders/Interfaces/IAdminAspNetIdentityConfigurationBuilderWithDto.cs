using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Builders.Interfaces
{
    public interface IAdminAspNetIdentityConfigurationBuilderWithDto<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>
        : IAdminAspNetIdentityConfigurationBuilder<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TRole : IdentityRole<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
    {
        new IAdminAspNetIdentityConfigurationBuilderWithDto<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole> UseDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure);
        new IAdminAspNetIdentityConfigurationBuilderWithDtoAndIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext> UseIdentityDbContext<TIdentityDbContext>() where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>;
        new IAdminAspNetIdentityConfigurationBuilderWithDtoAndPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext> UsePersistedGrantDbContext<TPersistedGrantDbContext>() where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext;
    }

    public interface IAdminAspNetIdentityConfigurationBuilderWithDtoAndIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext>
        : IAdminAspNetIdentityConfigurationBuilderWithDto<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>
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
        new IAdminAspNetIdentityConfigurationBuilderWithDtoAndIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TIdentityDbContext> UseDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure);
        new IAdminAspNetIdentityConfigurationBuilderWithDtoAndIdentityDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TNewIdentityDbContext> UseIdentityDbContext<TNewIdentityDbContext>() where TNewIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>;
        new IConfiguredAdminAspNetIdentityConfigurationBuilder<TKey> UsePersistedGrantDbContext<TPersistedGrantDbContext>() where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext;
    }

    public interface IAdminAspNetIdentityConfigurationBuilderWithDtoAndPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext>
        : IAdminAspNetIdentityConfigurationBuilderWithDto<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole>
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
        new IAdminAspNetIdentityConfigurationBuilderWithDtoAndPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TPersistedGrantDbContext> UseDto(Action<IAdminAspNetIdentityDtoConfigurationBuilder<TKey>> configure);
        new IAdminAspNetIdentityConfigurationBuilderWithDtoAndPersistedGrantDbContext<TKey, TUser, TUserClaim, TUserLogin, TUserToken, TRole, TRoleClaim, TUserRole, TNewPersistedGrantDbContext> UsePersistedGrantDbContext<TNewPersistedGrantDbContext>() where TNewPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext;
        new IConfiguredAdminAspNetIdentityConfigurationBuilder<TKey> UseIdentityDbContext<TIdentityDbContext>() where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>;
    }
}