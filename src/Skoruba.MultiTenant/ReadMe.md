# Skoruba.MultiTenant

Skoruba.Multitenant provides an abstraction for multi-tenancy along with ASP.NET
and Identity Server 4 support.  An example implementation is provided 
using [Finbuckle.MultiTenant](https://www.finbuckle.com/MultiTenant)
for tenant resolution which includes several resolution strategies including
the ability to create custom strategies.  

## PR impact on Admin and STS projects
One of the goals of this implementation is to minimize the number of changes
required within Admin and STS when enabling or disabling multitenancy.  The 
following outlines noteable concessions:

- The Profile resource needs to include "TenantId" claim.
- A TenantId/TenantCode property was included in a few default objects like
the Role and User entity objects and the login and register viewmodels.  Single tenant 
developers can ignore these properties, or they can remove them if they wish, 
but that is not necessary.  There may need to be additional changes 
like this to dto's and other objects when
more tenant enhancements are added, such as tenant pages for adding/editing
tenants.
- A static flag MultiTenantEnabled is referenced in various places during setup
for configuration purposes.  This flag currently resides in the Skoruba.MultiTenant 
project and must be changed when deciding on being multitenant or not.
- The AdminIdentityDbContext uses a const flag to modify indexes if
multitenancy is enabled.  This is essentially a non-change.
- Skoruba.Dbmigrator is a console project to make it easier to migrate and
seed.  The existing migration code was left alone and can still be utilizied;
howeer, DbMigrator must be used for the current Finbuckle implemenation.  More
on this below.

- User and Role stores were added to the EF Shared project for ASP.NET Identity.
These stores are used to override the default ASP.NET Identity stors.

## Using Skoruba.MultiTenant.Finbuckle
To implement multitenancy you must add services and middleware.  

### Admin Project
The following line will register the required services and return a builder object
specific for Finbuckle.  

```c#
services.AddMultiTenant(true);
```

From there you can register the tenant store and strategy.  See 
[Finbuckle.MultiTenant](https://www.finbuckle.com/MultiTenant) for details.
Skoruba.MultiTenant.Finbuckle includes a claims strategy and a FormStrategy which are not provided
by Finbuckle.  But you can use any of the Finbuckle strategies and stores.

```C#
// Add multitenancy
services.AddMultiTenant(true)
    // custom store
    .WithEFCacheStore(options => options.UseSqlServer(Configuration.GetConnectionString("TenantsDbConnection")))
    // custom strategy to get tenant id from user claims
    .WithStrategy<ClaimsStrategy>(ServiceLifetime.Singleton)
    ;
```

To configure the middleware add this line in the Startup.  Becuase the ClaimsStrategy 
requires authentication middleware to run first, we add the following line after 
adding authentication. Other strategies may require this line to come before
authentication.  Review the Finbuckle docs for more details.

```c#
// configure multitenant middleware after authentication when the strategy is to use claims
// note: other strategies may require the configuration to come before authentication.
app.UseMultiTenant();
```

### STS Project
The STS project is a little different configuration.  This is becuase we are adding
an addditional strategy to resolve the tenant, FormStrategy.  The FormStrategy
resolves the tenant from the POST form.  These are configuration dependent.

```c#
    // If single tenant app then change to false and remove app configuration
    services.AddMultiTenant(true)
        // required if using app.AddMultiTenantFromForm()
        .RegisterConfiguration(Configuration.GetSection("MultiTenantConfiguration"))
        // custom store
        .WithEFCacheStore(options => options.UseSqlServer(Configuration.GetConnectionString("TenantsDbConnection")))
        // custom strategy to get tenant from form data at login
        .WithStrategy<FormStrategy>(ServiceLifetime.Singleton)
        // dont require tenant resolution for identity endpoints
        .RegisterTenantIsRequiredValidation<TenantNotRequiredForIdentityServerEndpoints>()
    ;
```
In the above we are also registering IValidateTenantRequirement.  This is being registered in
STS because the Identity Server 4 endpoints do not require tenant resoltuion.  In the Admin project
this is not added becuase the admin project requires the tenant to always be resolved.

For middleware we are doing this differently in STS.  The reason is becuase we need to
resolve the tenant with the FormStrategy as well as the ClaimStrategy.  But the claim stategy
must come after the authentication.  So this is what it looks like:

```c#
// configure default multitenant middleware before authentication
app.UseMultiTenant();

UseAuthentication(app);

// configure custom multitenant middleware for claims after authentication
app.UseMultiTenantFromClaims();
``` 

## Configuring for single tenant
Three things need to occur.

First, set the MultiTenantEnable constant to false in Skoruba.MultiTeant.

Second, change the services line to false and remove all of the other service configurations.

```C#
// Add multitenancy
services.AddMultiTenant(false);
```

And third, delete or comment the middleware for multitenancy.


## Implementing Skoruba.MultiTenant

### ISkorubaMultiTenant
Various objects within the Skoruba infrastructure depend on this object.  This
object defines the tenant properties.  A sample implementation of this is in
the Finbuckle project and illustrates how ISkorubaTenant "wraps" the Finbuckle
TenantInfo object. 

### IValidateTenantRequirement
This allows some code to execute without a resolved tenant.  For instance,
if the user table is a single table for all tenants, it is not necessary
for Identity Server endpoints to know the tenant. However, if you use
a domain or url strategy for resolving the tenant then you may be able
to reliably define the tenant for each endpoint.  In such scenarios this
object may not need to be implementated.

## ASP.NET Identity
Tenant specific user tables is difficult (impossible?) unless a domain or url strategy
is implementated.  The current implementation with Finbuckle does not address
either of these strategies but it should be possible.

The current example implementation uses a single user table with a TenantId
property.  In order to ensure data isolation a new UserStore and RoleStore
were implemented.  These are abstracted in the Skoruba.MultiTenant.Identity 
project and implementd in the Skoruba.IdentityServer4Admin.EndityFramework.Shared
project.

It is currently assumed that the User and Role are tenant specific.  A tenant
may define unique roles for their implementation.

## Notes

### Skoruba.MultiTenant.EfCacheStore
This project is a custom tenant store.  The Finbuckle tenant store for dbcontext
does not include code for storing a dictionary of items.  This store also
implements a local cache (which could be modified for distributed cache).

### DbMigrator
Tenancy relies on middleware to resolve the tenant which is then injected with DI
on demand.  The MultiTenantUserStore and MultiTenantRoleStore both require the
tenant id in order to relizbly add or update data.  And these stores rely on Di, it
is not possible to set the tenant.

Seed data should also allow for seeding users and roles in different tenants.
This requires the ability to add a tenant to the IServiceCollection while seeding.

The DbMigrator defines a IMigrateAndSeed which is implemented per dbcontext.  It
does not matter where each implementation resides, but it is necessary for the 
implementation to be registered in the AddMigrateAndSeedServices in the Program
file.  

To run the migrator set it as the startup project and run it.