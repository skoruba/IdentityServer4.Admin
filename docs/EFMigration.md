# Create the migration of database:

```
Add-Migration Initial -context AdminDbContext -output Data/Migrations
Update-Database -context AdminDbContext
```

# Using other database engines


## PostgreSQL

Install following NuGet package:

```
Npgsql.EntityFrameworkCore.PostgreSQL.Design
```

Find `RegisterDbContexts` function in `Helpers\StartupHelpers.cs`

```csharp
services.AddDbContext<AdminDbContext>(options => options.UseSqlServer(configuration.GetConnectionString(ConfigurationConsts.AdminConnectionStringKey), optionsSql => optionsSql.MigrationsAssembly(migrationsAssembly)));
```

and change  `UseSqlServer` to `UseNpgsql`.

**Don't forget to update your connection string in appsettings.json and (re)generate migrations for new database**


## SQLite


Install following NuGet package:

```
Microsoft.EntityFrameworkCore.Sqlite.Design
```

Find `RegisterDbContexts` function in `Helpers\StartupHelpers.cs`

```csharp
services.AddDbContext<AdminDbContext>(options => options.UseSqlServer(configuration.GetConnectionString(ConfigurationConsts.AdminConnectionStringKey), optionsSql => optionsSql.MigrationsAssembly(migrationsAssembly)));
```

and change  `UseSqlServer` to `UseSqlite`.

**Don't forget to update your connection string in appsettings.json and (re)generate migrations for new database**

