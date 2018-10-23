# How to use existing IdentityServer4 instance

- You can use one or more `DbContexts` for the administration.

- The configuration of DbContexts is in the `Startup.cs`:

## 1) Single DbContext:

```cs
services.AddAdminServices<AdminDbContext>();
```

`AddAdminServices` expects one generic param:

- `TAdminDbContext` - It requires to implement interfaces `IAdminConfigurationDbContext`, `IAdminPersistedGrantDbContext`, `IAdminLogDbContext`

## 2) Multiple DbContexts:

- It is possible to overload this method:

```cs
services.AddAdminServices<TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext>();
```

`AddAdminServices` expects following generic params:

- `TConfigurationDbContext` - DbContext for the configuration data
  - It requires to implement interface `IAdminConfigurationDbContext`
- `TPersistedGrantDbContext` - DbContext for the operational data
  - It requires to implement interface `IAdminPersistedGrantDbContext`
- `TLogDbContext` - for the logs - DbContext for the operational data
  - It requires to implement interface `IAdminLogDbContext`
