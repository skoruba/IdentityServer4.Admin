![Logo](docs/Images/Skoruba.IdentityServer4.Admin-Logo-ReadMe.png)
# Skoruba.IdentityServer4.Admin

> The administration of the IdentityServer4 and Asp.Net Core Identity

## Project Status

[![Build status](https://ci.appveyor.com/api/projects/status/5yg59bn70399hn6s/branch/master?svg=true)](https://ci.appveyor.com/project/JanSkoruba/identityserver4-admin/branch/master)

This is currently in **beta version**

The application is written in the **Asp.Net Core MVC - using .NET Core 2.0** - works only with **IdentityServer4 version 2.0+**

## Cloning

```sh
git clone https://github.com/skoruba/IdentityServer4.Admin
```

## Installation of the Client Libraries

```sh
cd src/Skoruba.IdentityServer4.Admin
npm install
```

## Bundling and Minification

The following Gulp commands are available:

- `gulp fonts` - copy fonts to the `dist` folder
- `gulp styles` - minify CSS, compile SASS to CSS
- `gulp scripts` - bundle and minify JS
- `gulp clean` - remove the `dist` folder
- `gulp build` - run the `styles` and `scripts` tasks

## EF Core & Data Access

- Run entity framework migrations - for instance from Visual Studio command line:

```powershell
Add-Migration DbInit -context AdminDbContext -output Data/Migrations
Update-Database -context AdminDbContext
```

Migrations are not a part of the repository - they are ignored in `.gitignore`.

We suggest to use seed data:

- In `Program.cs` -> `Main`, uncomment `DbMigrationHelpers.EnsureSeedData(host)`
- The `Clients` and `Resources` files in `Configuration` are the initial data, based on a sample from IdentityServer4

## Authentication and Authorization

- `Constants/AuthorizationConsts.cs` contains configuration constants
- In the controllers, `AuthorizationConsts.AdministrationPolicy` is used and by default the `AuthorizationConsts.AdministrationRole` is required
- With the default configuration, it is necessary to configure and run instance of IdentityServer4. It is possible to use initial migration for creating the client as it mentioned above
    - Change the specific URLs and names for the IdentityServer in `Constants/AuthorizationConsts`

## Localizations - labels, messages

- All labels and messages are stored in the resources `.resx` - locatated in `/Resources`
    - Client label descriptions from - http://docs.identityserver.io/en/release/reference/client.html
    - Api Resource label descriptions from - http://docs.identityserver.io/en/release/reference/api_resource.html
    - Identity Resource label descriptions from - http://docs.identityserver.io/en/release/reference/identity_resource.html

## Overview

- Solution structure:
	- `Skoruba.IdentityServer4` - Quickstart UI for an in-memory IdentityServer4 v2 (for development) - (https://github.com/IdentityServer/IdentityServer4.Quickstart.UI)
	- `Skoruba.IdentityServer4.Admin` - ASP .NET Core MVC application that contains Admin UI
	- `Skoruba.IdentityServer4.Admin.IntegrationTests` - xUnit project that contains the integration tests
	- `Skoruba.IdentityServer4.Admin.UnitTests` - xUnit project that contains the unit tests

- The admininistration contains the following sections:

![Skoruba.IdentityServer4.Admin App](docs/Images/Skoruba.IdentityServer4.Admin-Solution.png)

## IdentityServer4
    
**Clients**

It is possible to define the configuration according the client type - by default the client types are used:

- Empty
- Web Application - Server side - Implicit flow
- Web Application - Server side - Hybrid flow
- Single Page Application - Javascript - Implicit flow
- Native Application - Mobile/Desktop - Hybrid flow
- Machine/Robot - Resource Owner Password and Client Credentials flow

- Actions: Add, Update, Clone, Remove
- Entities:
    - Client Cors Origins
    - Client Grant Types
    - Client IdP Restrictions
    - Client Post Logout Redirect Uris
    - Client Properties
    - Client Redirect Uris
    - Client Scopes
    - Client Secrets

- **API Resources**

- Actions: Add, Update, Remove
- Entities: 
    - Api Claims
    - Api Scopes
    - Api Scope Claims
    - Api Secrets

- **Identity Resources**

- Actions: Add, Update, Remove
- Entities:
    - Identity Claims

## Asp.Net Core Identity

**Users**

- Actions: Add, Update, Delete
- Entities:
    - User Roles
    - User Logins
    - User Claims

**Roles**

- Actions: Add, Update, Delete
- Entities:
    - Role Claims

## Application Diagram

![Skoruba.IdentityServer4.Admin Diagram](docs/Images/Skoruba.IdentityServer4.Admin-App-Diagram.png)

## Plan & Vision

- Add more unit and integration tests :blush:
- Extend administration for another protocols

## Licence

This repository is licensed under the terms of the [**MIT license**](LICENSE.md).

## Acknowledgements

This web application is based on these projects:

- ASP.NET Core
- IdentityServer4.EntityFramework
- ASP.NET Core Identity
- XUnit
- Fluent Assertions
- Bogus
- AutoMapper
- Serilog

Thanks to [Tomáš Hübelbauer](https://github.com/TomasHubelbauer) for the initial code review.

Thanks to [Dominick Baier](https://github.com/leastprivilege) and [Brock Allen](https://github.com/brockallen) - the creators of IdentityServer4.

## Contact and Suggestion

I am happy to share my attempt of the implementation of the administration for IdentityServer4 and ASP.NET Core Identity.

Any feedback is welcome - feel free to create an issue or send me an email - [jan@skoruba.com](mailto:jan@skoruba.com). Thank you :blush:
