# Skoruba.DbMigrator
The Skrouba.DbMigrator is a console app that will perform the migrations
for your Entity Framework DbContexts.  

The reason this project was developed was because there was a need
to be able to utilize dependency injection while migrating.  Specifically,
the implementation of multi-tenancy includes services that rely on 
knowing a tenant.  And the only way to do that is to add the tenant
to the service collection.

Another reason is that this approach allows for more extensibility.  A
developer can create any number of migration objects that implement
the IMigrateAndSeed interface and those objects will execute.  This also
allows contributors to provide alternative migration implementations.

## Overview
DbMigrator is a console app and can easily be run from within Visual
Studio.

DbMigrator uses DI to register services and configurations to execute
your migration.  It will use the Skoruba.IdentityServer4.Admin.Startup
class as the source for configuration and service registration.

DbMigrator uses Scrutor to scan all of the assemblies that it references and 
will build a list of IMigrateAndSeed objects.  Scrutor can only find
implementations that the project has reference to.  The advantage is
that there is no need to manually register these implementations.

## Adding an IMigrateAndSeed implementation
Create a class and implement the IMigrateAndSeed interface.  It doesn't
matter where you create this class so long as the DbMigrator project 
has a reference to the assembly the implemetation resides in.

In the Migrate method, return the base method for migrating.  The base
method will get your dbcontext from the service container and then
call await context.Database.MigrateAsync() for your dbContext.

In the Seed method create your logic for seeding.  There's not a lot of
magic here.  

There's several implementations of this provided by default.

## DependsOn
*Note: This attribute should not be necessary.  If your DbContext requires
other DbContexts to execute first then you may have a problem.  Nevertheless,
here it is.*

The DependsOn attribute is a helper attribute that allows you to specify
your implementation requires another migration to run first.  Behind the
scenes DbMigrator finds all of the IMigrateAndSeed objects and then
uses the DependsOnAttribute to put them in order so that dependencies
are executed first.

The attribute accepts typeof() or strings representing the class full name.
The use of strings allows you to add a dependency to another migration
that is not referenced by your project.

Circular referneces will throw an error.


