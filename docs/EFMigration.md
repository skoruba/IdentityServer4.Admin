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

## MySQL and MariaDB


Install the following NuGet package:
```
Pomelo.EntityFrameworkCore.MySql
```

Find `RegisterDbContexts` function in `Helpers\StartupHelpers.cs`

```csharp
services.AddDbContext<AdminDbContext>(options => options.UseSqlServer(configuration.GetConnectionString(ConfigurationConsts.AdminConnectionStringKey), optionsSql => optionsSql.MigrationsAssembly(migrationsAssembly)));
```

and change  `UseSqlServer` to `UseMySql`.

Find `Properties` in `Skoruba.IdentityServer4.Admin.EntityFramework\Entities\Log.cs`

```csharp
[Column(TypeName = "xml")]
public string Properties { get; set; }
```

and remove the `[Column]` attribute. As MySQL and MariaDB don't know about a XML data type.

**Don't forget to update your connection string in appsettings.json and (re)generate migrations for new database**
