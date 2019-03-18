# How to configure the Administration - IdentityServer4 and Asp.Net Core Identity

## 1) Admin UI:

- `Skoruba.IdentityServer4.Admin` - `Startup.cs` - method `ConfigureServices`:

### Configure DbContexts

- This `AddDbContexts` helper method is used for registration of DbContexts for whole administration.

- The solution uses these `DbContexts`:

  - `AdminIdentityDbContext`: for Asp.Net Core Identity
  - `AdminLogDbContext`: for logging
  - `IdentityServerConfigurationDbContext`: for IdentityServer configuration store
  - `IdentityServerPersistedGrantDbContext`: for IdentityServer operational store

```
services.AddDbContexts<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>(HostingEnvironment, Configuration);

```

### Configure authentication

```
services.AddAuthenticationServices<AdminIdentityDbContext, UserIdentity, UserIdentityRole>(HostingEnvironment, rootConfiguration.AdminConfiguration);
```

This `AddAuthenticationServices` helper method is for registration authentication. For administration is used OpenIdConnect middleware which is connected to IdentityServer4.

> For staging environment is used cookie middleware for fake authentication. In integration tests is checked this fake login url. /Account/Login

### Configuration of services/repositories for IdentityServer4

```
services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();
```

This extension method `AddAdminServices` is for registration all dependencies - like repositories, services for managing IdentityServer4 configuration and operational store. Here is necessary to inject DbContexts only.

### Configuration of Asp.Net Core Identity

```
services.AddAdminAspNetIdentityServices<AdminIdentityDbContext, IdentityServerPersistedGrantDbContext, UserDto<string>, string, RoleDto<string>, string, string, string,
                                UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
                                UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
                                UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string, string>,
                                UserClaimsDto<string>, UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>,
                                RoleClaimsDto<string>, UserClaimDto<string>, RoleClaimDto<string>>();
```

This extension method is for registration all dependencies for managing data for Asp.Net Core Identity.
This is right place for changing Identity model - like change primary key from `string` to another type.

### Configuration of Localization and MVC

```
services.AddMvcWithLocalization<UserDto<string>, string, RoleDto<string>, string, string, string,
                UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
                UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
                UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string, string>,
                UserClaimsDto<string>, UserProviderDto<string>, UserProvidersDto<string>, UserChangePasswordDto<string>,
                RoleClaimsDto<string>>();
```

This extension method `AddMvcWithLocalization` is for registration of MVC and Localization as well.
In this method are used same types like for Asp.Net Core Identity because these types are injected into generic Controllers.

### Configuration of Authorization policies

```
services.AddAuthorizationPolicies();
```

This extensions method contains only one base policy for administration of whole AdminUI. There is good place to register another policies for extending authorization stuff.

## 2) Security token service (STS)

- `Skoruba.IdentityServer4.STS.Identity` - `Startup.cs` - method `ConfigureServices`:

### Configure DbContexts

```
services.AddIdentityDbContext<AdminIdentityDbContext>(Configuration);
```

In this extension method `AddIdentityDbContext` is defined DbContext for Asp.Net Core Identity

In `StartupHelpers.cs` is another extension method for registration of DbContexts for IdentityServer4:

```
AddIdentityServerStoresWithDbContexts<TConfigurationDbContext, TPersistedGrantDbContext>(configuration);
```

### Configuration of IdentityServer4 and Asp.Net Core Identity

- `Skoruba.IdentityServer4.STS.Identity` - `Startup.cs` - method `ConfigureServices`:

```
services.AddAuthenticationServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminIdentityDbContext, UserIdentity, UserIdentityRole>(Environment, Configuration, Logger);
```

- This extension method is defined for registration of Asp.Net Core Identity and IdentityServer4 - including one external provider (GitHub).

### Configuration of Localization and MVC

```
services.AddMvcWithLocalization<UserIdentity, string>();
```

- This is extension method for registraion of MVC and Localization. In this method are used the types for Asp.Net Core Identity for generic controllers.
