# Skoruba.MultiTenant
Skoruba.MultiTenant is an abstraction for multi-tenant support within the Skoruba.IdentityServer4
infrastructure.  It allows custom tenant resolution strategies as well as support
for Identity and Identity Server.

A sample implementation is provided using [Finbuckle.MultiTenant](https://www.finbuckle.com/MultiTenant)
using the project Skoruba.MultiTenant.Finbuckle.

## Overview
Multi-tenancy works by injecting a ISkorubaTenantContext wherever a tenant needs
to be known.  By injecting this object developers can use the TenantId to filter
data to ensure data isolation.  The developer is responsible to create an object that 
implements this interface and defines the various context properties including
the tenant information.

Configuring the solution for multi-tenancy is kept simple through the appsetting
MultiTenantConfiguration:MultiTenantEnabled.  False will maintain a single-tenant
solution and true will maintain a multi-tenant solution.

Single tenant solutions need no further changes. Multi tenant solutions will 
require the developer to implement the necessary 
services and middleware to resolve and provide the tenant information.

Services are available for Asp.Net Identity to help with data isolation.  These services
will make use of the resolved tenant and filter any underlying queries.  They also
allow users to belong to multiple tenants with the same username/email as well
as roles to be specific to each tenant.

## Basics
To configure the solution for multi-tenancy set the appsetting 
MultiTenantConfiguration:MultiTenantEnabled to true.

Next, configure the STS and Admin projects.  The configuration only
needs to be done in the Startup class and only if MultiTenantEnabled is true.
The Startup class has three virtual methods that need to be configured.  These are
virtual so that they can be overriden for integration tests.

- **RegisterMultiTenantConfiguration** This method is used to register the 
services for the tenant resolution strategy. *Note that any changes here should 
be considered for changes to the Skoruba.IdentityServer4.Admin.Configuration.Test.StartupTestMultiTenant 
class.*

- **UsePreAuthenticationMultitenantMiddleware** This method is used to configure 
any middleware that needs to be configured BEFORE the authentication middleware.

- **UsePostAuthenticationMultitenantMiddleware** This method is used to configure
any middleware that needs to be configured AFTER the authentication middleware.

- **TODO:** startup helpers that register additional services may need to be modified
depending on a chosen implementation.

## Default Configuration Deep Dive
There's a lot going on, so let's break down how the default implementation is
configured and why.  Keep in mind that this is an opinionated implementation.

### Use Case
The use case of the default implementation is:

#### To support multiple tenants for a single application
This implementation is not focused on client or api resource tenant isolation.

#### To maintain a single url (no domain or route urls for tenants)
No configuration is required for this one.  This is part of the use case so that we
explicitly clarify that we are <ins>not</ins> using a domain or route strategy.  These are 
common strategies, but we are <ins>not</ins> using them for this example.

#### Isolate users and roles so that user can belong to multiple tenants with the same username/email as well as allow tenants to have unique roles
To accomplish these we'll need to make sure we isolate our user and roles tables
with a TenantId.  The TenantId is already added to the entities and part of the db,
but in order to make sure we keep tenants in their own data we'll register 
MultiTenantUserStore and MultiTenantRoleStore.
These stores will replace the default Asp.Net Identity stores and will use the resolved
TenantId from ISkorubaTenantContext to filter any queries.

#### Keep the login simple
Users who belong to multiple tenants should not have to remember url's or domains
in order to login.  And once the user is logged in the application should be able
to know what tenant the user belongs to.

In order to keep it simple we're going to use a company code that the user will
include during login.  The company code will resolve the tenant using the FormStrategy.
After login we will use claims to determine the tenant which will be resolved
using the ClaimsStrategy or the MultiTenantClaimMiddleware.

We're going to make use of some additional services to make our lives easier.

- MultiTenantUserClaimsPrincipalFactory will automatically create a claim for the
user's TenantId and add it to the db.

- MultiTenantProfileService will make sure that all Identity Server profile requests
include the users TenantId as a claim.  *(Note: This may not be necessary since
we're using the MultiTenantUserClaimsPrincipalFactory.  Testing is required to confirm.)*

- TenantNotRequiredForIdentityServerEndpoints is a service that compares the url to
the IdentityServer endpoints, and if the request is going to one of those endpoints
the ISkorubaTenantContext will indicate that resolving the tenant is not required.  This
will allow the MultiTenantUserStore and MultiTenantRoleStore to exclude TenantId
filtering.
  
### Configuring STS
##### appsetting 
In the appsettings set the MultiTenantConfiguration:MultiTenantEnabled to true.

##### Startup method RegisterMultiTenantConfiguration

```cs
var configuration = Configuration.GetSection(ConfigurationConsts.MultiTenantConfiguration).Get<MultiTenantConfiguration>();
services.AddMultiTenantConfiguration<SkorubaTenantContext>(configuration)
    .RegisterTenantIsRequiredValidation<TenantNotRequiredForIdentityServerEndpoints>()
    .WithFinbuckleMultiTenant(Configuration.GetSection(ConfigurationConsts.MultiTenantConfiguration))
    .WithEFCacheStore(options => options.UseSqlServer(Configuration.GetConnectionString("TenantsDbConnection")))
    .WithStrategy<FormStrategy>(ServiceLifetime.Singleton);
```
- **services.AddMultiTenantConfiguration<SkorubaTenantContext>(configuration)** Registers
the SkorubaTenantContext which implements the ISkorubaTenantContext.  This class wraps the
Finbuckle TenantInfo.  The configuration is added to the service so that it can be injected 
with the SkorubTenantContext and other areas of the application.  This configuration 
allows configuration program execution to change depending on being single or multi-tenant.

- **.RegisterTenantIsRequiredValidation<TenantNotRequiredForIdentityServerEndpoints>()** Registers
the service to not require tenant resolution for Identity Server endpoints.

- **.WithFinbuckleMultiTenant(Configuration.GetSection(ConfigurationConsts.MultiTenantConfiguration))**
Registers the Finbuckle.MultiTenant implementation.  It also uses the settings in the IConfigurationSection
to know about what actions and form values will be used in the FormStrategy to determine
the tenant.

- **.WithEFCacheStore(options => options.UseSqlServer(Configuration.GetConnectionString("TenantsDbConnection")))**
Registers a custom store per Finbuckle instructions.  The store we're implementing caches
tenants for 48 hours.  You can use any store you want.  Finbuckle has several out of the box.

- **.WithStrategy<FormStrategy>(ServiceLifetime.Singleton);** Registers the Finbuckle
strategy we're going to primarily use to resolve the tenant.  Resolving using claims will
come later.

##### Startup method UsePreAuthenticationMultitenantMiddleware
Since our resolution strategy is the FormStrategy we are going simply add the default
Finbuckle middleware here.

##### Startup method UsePostAuthenticationMultitenantMiddleware
This is where we need to configure middleware for using claims.  Claim resolution 
strategies must come after authentication middleware.

##### Notes about StartupHelpers
You dont need to do anything in this class.  The flag for MultiTenantEnabled will be
used to add (or not) additional services.


### Configuring Admin
##### appsetting 
In the appsettings set the MultiTenantConfiguration:MultiTenantEnabled to true.

##### Startup method RegisterMultiTenantConfiguration

```cs
var configuration = Configuration.GetSection(ConfigurationConsts.MultiTenantConfiguration).Get<MultiTenantConfiguration>();
services.AddMultiTenantConfiguration<SkorubaTenantContext>(configuration)
    .WithFinbuckleMultiTenant()
    .WithEFCacheStore(options => options.UseSqlServer(Configuration.GetConnectionString("TenantsDbConnection")))
    .WithStrategy<ClaimsStrategy>(ServiceLifetime.Singleton);
```
- **services.AddMultiTenantConfiguration<SkorubaTenantContext>(configuration)** Registers
the SkorubaTenantContext which implements the ISkorubaTenantContext.  This class wraps the
Finbuckle TenantInfo.  The configuration is added to the service so that it can be injected 
with the SkorubTenantContext and other areas of the application.  This configuration 
allows configuration program execution to change depending on being single or multi-tenant.

- **Note:** we are not registering the TenantNotRequiredForIdentityServerEndpoints service
becuase this is the admin project and we dont have those endpoints here.

- **.WithFinbuckleMultiTenant()**
Registers the Finbuckle.MultiTenant implementation.  We dont need any configuration settings
so we're not passing in the IConfigurationSection.

- **.WithEFCacheStore(options => options.UseSqlServer(Configuration.GetConnectionString("TenantsDbConnection")))**
This store should be the same as the STS store.  

- **.WithStrategy<ClaimsStrategy>(ServiceLifetime.Singleton);** Registers the strategy
to resolve tenants using the user claims.  With this strategy we have to add our middleware
after authentication.

##### Startup method UsePreAuthenticationMultitenantMiddleware
Since our resolution strategy is the ClaimsStrategy we are not going to configure any 
middleware here.

##### Startup method UsePostAuthenticationMultitenantMiddleware
This is where we need to configure our middleware.  *NOTE: The UseMultiTenantFromClaims middleware
is only used after the UseMultiTenant middleware is configured.  Think of the 
UseMultiTenantFromClaims as a backup middleware if the UseMultiTenant middleware does not
resolve the tenant.**

##### Notes about StartupHelpers
You dont need to do anything in this class.  The flag for MultiTenantEnabled will be
used to add (or not) additional services.

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