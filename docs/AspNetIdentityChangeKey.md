# How to configure Identity primary key data type in ASP.NET Core Identity

- By default, it's used `int` as the primary key, but you can change to `Guid` or `string` etc.

## How to use for example `Guid`:

### 1. Change `int` to `Guid` in `Startup.cs`:

Original:

```cs
services.AddAdminServices<AdminDbContext, UserDto<int>, int, RoleDto<int>, int, int, int,
                UserIdentity, UserIdentityRole, int, UserIdentityUserClaim, UserIdentityUserRole,
                UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>();
```

New:

```cs
services.AddAdminServices<AdminDbContext, UserDto<Guid>, Guid, RoleDto<Guid>, Guid, Guid, Guid,
                UserIdentity, UserIdentityRole, Guid, UserIdentityUserClaim, UserIdentityUserRole,
                UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>();
```

### 2. Change `int` to `Guid` in all files in folder - `Skoruba.IdentityServer4.Admin.EntityFramework/Entities/Identity`:

For example - `UserIdentity.cs`:

Original:

```cs
public class UserIdentity : IdentityUser<int>
	{

	}
```

New:

```cs
public class UserIdentity : IdentityUser<Guid>
	{

	}
```

- Change `int` to `Guid` in other files in this folder - `Skoruba.IdentityServer4.Admin.EntityFramework/Entities/Identity`

### 3. Change `int` to `Guid` in all files in folder - `Skoruba.IdentityServer4.Admin/Views/Identity`:

For example - `Role.cshtml`:

Original:

```cs
@model Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity.RoleDto<int>

...

@if (!EqualityComparer<int>.Default.Equals(Model.Id, default(int)))

...
```

New:

```cs
@model Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity.RoleDto<Guid>

...

@if (!EqualityComparer<Guid>.Default.Equals(Model.Id, default(Guid)))

...
```

- Change `int` to `Guid` in other files in this folder - `Skoruba.IdentityServer4.Admin/Views/Identity`

### 4. Change `int` to `Guid` in `AdminDbContext` - `Skoruba.IdentityServer4.Admin.EntityFramework/DbContexts`:

Original:

```cs
public class AdminDbContext : IdentityDbContext<UserIdentity, UserIdentityRole, int, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>,
        IAdminConfigurationDbContext, IAdminLogDbContext, IAdminPersistedGrantIdentityDbContext
```

New:

```cs
public class AdminDbContext : IdentityDbContext<UserIdentity, UserIdentityRole, Guid, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>,
        IAdminConfigurationDbContext, IAdminLogDbContext, IAdminPersistedGrantIdentityDbContext
```

### 5. Change `int` to `Guid` in `GrantController` - `Skoruba.IdentityServer4.Admin/Controllers`:

Original:

```cs
public class GrantController : BaseController
    {
        private readonly IPersistedGrantService<AdminDbContext, UserIdentity, UserIdentityRole, int, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken> _persistedGrantService;
        private readonly IStringLocalizer<GrantController> _localizer;

        public GrantController(IPersistedGrantService<AdminDbContext, UserIdentity, UserIdentityRole, int, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken> persistedGrantService,
            ILogger<ConfigurationController> logger,
            IStringLocalizer<GrantController> localizer) : base(logger)
        {
            _persistedGrantService = persistedGrantService;
            _localizer = localizer;
        }
    }
```

New:

```cs
public class GrantController : BaseController
    {
        private readonly IPersistedGrantService<AdminDbContext, UserIdentity, UserIdentityRole, Guid, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken> _persistedGrantService;
        private readonly IStringLocalizer<GrantController> _localizer;

        public GrantController(IPersistedGrantService<AdminDbContext, UserIdentity, UserIdentityRole, Guid, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken> persistedGrantService,
            ILogger<ConfigurationController> logger,
            IStringLocalizer<GrantController> localizer) : base(logger)
        {
            _persistedGrantService = persistedGrantService;
            _localizer = localizer;
        }
    }
```

### 6. Change `int` to `Guid` in `IdentityController` - `Skoruba.IdentityServer4.Admin/Controllers`:

Original:

```cs
public class IdentityController : BaseIdentityController<AdminDbContext, UserDto<int>, int, RoleDto<int>, int, int, int, UserIdentity, UserIdentityRole, int, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>
    {
        public IdentityController(IIdentityService<AdminDbContext, UserDto<int>, int, RoleDto<int>, int, int, int, UserIdentity, UserIdentityRole, int, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken> identityService, ILogger<ConfigurationController> logger, IStringLocalizer<IdentityController> localizer)
            : base(identityService, logger, localizer)
        {
        }
    }
```

New:

```cs
public class IdentityController : BaseIdentityController<AdminDbContext, UserDto<Guid>, Guid, RoleDto<Guid>, Guid, Guid, Guid, UserIdentity, UserIdentityRole, Guid, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>
    {
        public IdentityController(IIdentityService<AdminDbContext, UserDto<Guid>, Guid, RoleDto<Guid>, Guid, Guid, Guid, UserIdentity, UserIdentityRole, Guid, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken> identityService, ILogger<ConfigurationController> logger, IStringLocalizer<IdentityController> localizer)
            : base(identityService, logger, localizer)
        {
        }
    }
```
