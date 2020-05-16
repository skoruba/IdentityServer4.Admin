# Multi-Tenant Clients & API Resources
These are notes about how a multi-tenant flow might work for clients and 
api resources.

This is a work in progress.

## Use Case
A person wants to provide an identity service that allows
multiple businesses (tenants) to authenticate their users
and protect their resources.

The tenant is able to register themselves and be able to self
manage their clients and resources.

A tenant creates a multi-tenant application and registers with the
identity service.  The tenant expects to be able to create specific
tenants for their application and to isolate their users per 
'application tenant'.

```
Definitions:
A 'tenant' as an individual
or business entity who uses the identity service for their 
own application(s).

An 'application tenant' is a sub-tenant whereby a tenant provides
an multi-tenant application.  
```

## Outcomes
- Tenants should have no knowledge of each other
- Tenant data should be isolated and/or separated
- The service should be scalable and not require manual intervention
every time a tenant registers.

## Methods to achieve outcomes
### Url Routing
When a tenant registers they are able to define a unique tenant
identifier.  
- The identifier must be unique across all tenants.  
- The identifier will be used for all of the tenants clients and resources
- The identifier must be url friendly. 
- The tenant will use the identifier in their application configuration

The STS will be configured to use the tenant identifier in the url
route.  This identifier will be used to lookup who the tenant is
in the tenants database and will then be available for service injection.

The STS will need to be able to create middleware at runtime which
can use the tenant-identifier to create and consume configuration values.


## Resources

- https://github.com/saaskit/saaskit/pull/96 describes per tenant middleware
that can be used in identity server to identify a tenant.

- https://www.finbuckle.com/MultiTenant/Docs/Options describes
an implementation of the options pattern allowing custom options
per tenant.
