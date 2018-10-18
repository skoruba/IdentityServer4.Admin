![Logo](docs/Images/Skoruba.IdentityServer4.Admin-Logo-ReadMe.png)

# Skoruba.IdentityServer4.Admin

> The administration of the IdentityServer4 and Asp.Net Core Identity

## Project Status

[![Build status](https://ci.appveyor.com/api/projects/status/5yg59bn70399hn6s/branch/master?svg=true)](https://ci.appveyor.com/project/JanSkoruba/identityserver4-admin/branch/master)
[![Join the chat at https://gitter.im/skoruba/IdentityServer4.Admin](https://badges.gitter.im/skoruba/IdentityServer4.Admin.svg)](https://gitter.im/skoruba/IdentityServer4.Admin?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

This is currently in **beta version**

The application is written in the **Asp.Net Core MVC - using .NET Core 2.1** - works only with **IdentityServer4 version 2.0+**

- [Install](https://www.microsoft.com/net/download/windows#/current) the latest .NET Core 2.x SDK

## Installation via dotnet new template

- Install the dotnet new template:

```sh
dotnet new -i Skoruba.IdentityServer4.Admin.Templates::1.0.0-beta4
```

- Create new project:

```sh
dotnet new skoruba.is4admin --name MyProject --title MyProject --adminrole MyRole --adminclientid MyClientId
```

Project template options:

```
--name: [string value] for project name
--title: [string value] for title and footer of the administration in UI
--adminrole: [string value] for name of admin role, that is used to authorize the administration
--adminclientid: [string value] for client name, that is used in the IdentityServer4 configuration
```

### How to use existing IdentityServer4 instance

- [Follow these steps for setup project to use existing IdentityServer4 instance](docs/Configure-To-Existing-IS4.md)

### How to configure Asp.Net Core Identity - database, primary key data type

- By default, it's used as the primary key `int`, but it's possible to change it:

- [Follow these steps to configure Identity](docs/Configure-To-Existing-Identity.md)

### Template uses following list of nuget packages

- [Available nuget packages](https://www.nuget.org/profiles/skoruba)

## Administration UI preview

- This administration uses bootstrap 4

![Admin-preview](docs/Images/App/Skoruba-Home-Preview.PNG)

- Forms:

![Admin-preview-form](docs/Images/App/Skoruba-Forms-Preview.PNG)

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

- Run entity framework migrations - for instance from Visual Studio command line (Nuget package manager):

```powershell
Add-Migration DbInit -context AdminDbContext -output Data/Migrations
Update-Database -context AdminDbContext
```

- Or via `dotnet CLI`:

```powershell
dotnet ef migrations add DbInit -c AdminDbContext -o Data/Migrations
dotnet ef database update -c AdminDbContext
```

Migrations are not a part of the repository - they are ignored in `.gitignore`.

### We suggest to use seed data:

- In `Program.cs` -> `Main`, uncomment `DbMigrationHelpers.EnsureSeedData(host)` or use dotnet CLI `dotnet run /seed`
- The `Clients` and `Resources` files in `Configuration/IdentityServer` are the initial data, based on a sample from IdentityServer4
- The `Users` file in `Configuration/Identity` contains the default admin username and password for the first login

### Using other database engines - PostgreSQL, SQLite, MySQL etc.

- [Follow these steps for setup other database engines](docs/EFMigration.md)

## Authentication and Authorization

- Change the specific URLs and names for the IdentityServer and Authentication settings in `Constants/AuthenticationConsts` or `appsettings.json`
- `Constants/AuthorizationConsts.cs` contains configuration of constants connected with authorization - definition of the default name of admin policy
- In the controllers is used the policy which name is stored in - `AuthorizationConsts.AdministrationPolicy`. In the policy - `AuthorizationConsts.AdministrationPolicy` is defined required role stored in - `AuthorizationConsts.AdministrationRole`.
- With the default configuration, it is necessary to configure and run instance of IdentityServer4. It is possible to use initial migration for creating the client as it mentioned above

## Localizations - labels, messages

- All labels and messages are stored in the resources `.resx` - locatated in `/Resources`
  - Client label descriptions from - http://docs.identityserver.io/en/release/reference/client.html
  - Api Resource label descriptions from - http://docs.identityserver.io/en/release/reference/api_resource.html
  - Identity Resource label descriptions from - http://docs.identityserver.io/en/release/reference/identity_resource.html

## Overview

- Solution structure:

  - `Skoruba.IdentityServer4` - Quickstart UI for an in-memory IdentityServer4 (for development) - (https://github.com/IdentityServer/IdentityServer4.Quickstart.UI)

  - `Skoruba.IdentityServer4.AspNetIdentity` - [Quickstart UI for the IdentityServer4 with Asp.Net Core Identity and EF Core storage](https://github.com/IdentityServer/IdentityServer4.Samples/tree/release/Quickstarts/Combined_AspNetIdentity_and_EntityFrameworkStorage)

  - `Skoruba.IdentityServer4.Admin` - ASP.NET Core MVC application that contains Admin UI

  - `Skoruba.IdentityServer4.Admin.BusinessLogic` - project that contains Dtos, Repositories, Services and Mappers for the IdentityServer4

  - `Skoruba.IdentityServer4.Admin.BusinessLogic.Identity` - project that contains Dtos, Repositories, Services and Mappers for the Asp.Net Core Identity

  - `Skoruba.IdentityServer4.Admin.BusinessLogic.Shared` - project that contains shared Dtos and ExceptionHandling for the Business Logic layer of the IdentityServer4 and Asp.Net Core Identity

  - `Skoruba.IdentityServer4.Admin.EntityFramework` - EF Core data layer that contains Entities for the IdentityServer4

  - `Skoruba.IdentityServer4.Admin.EntityFramework.Identity` - EF Core data layer that contains Entities for the Asp.Net Core Identity

  - `Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts` - project that contains AdminDbContext for the administration

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

**API Resources**

- Actions: Add, Update, Remove
- Entities:
  - Api Claims
  - Api Scopes
  - Api Scope Claims
  - Api Secrets

**Identity Resources**

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

### 1.0.0:

- [x] Create the Business Logic & EF layers - available as a nuget package
- [ ] Create a project template using dotnet CLI - `dotnet new template`
  - [x] First template: The administration of the IdentityServer4 and Asp.Net Core Identity
  - [ ] Second template: The administration of the IdentityServer4 (without Asp.Net Core Identity) ([#79](https://github.com/skoruba/IdentityServer4.Admin/issues/79))
- [ ] Add audit logs to track changes ([#61](https://github.com/skoruba/IdentityServer4.Admin/issues/61))
- [x] Add localization for other languages
  - [x] English
  - [x] Chinese
- [ ] User registration / Password reset
- [ ] Account linking
- [ ] Manage profile

### Future:

- Add more unit and integration tests :blush:
- Extend administration for another protocols
- Create separate UI using `Razor Class Library`

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

## Contributors

Thanks goes to these wonderful people ([emoji key](https://github.com/kentcdodds/all-contributors#emoji-key)):

<!-- prettier-ignore-start -->
| [<img src="https://avatars3.githubusercontent.com/u/35664089?s=460&v=3" width="100px;"/><br /><sub> Jan Škoruba</sub>](https://github.com/skoruba) <br /> 💻 💬 📖 💡 🤔 | [<img src="https://avatars0.githubusercontent.com/u/6831144?s=460&v=3" width="100px;"/><br /><sub> Tomáš Hübelbauer</sub>](https://github.com/TomasHubelbauer) <br /> 💻 👀 📖  🤔 | [<img src="https://avatars0.githubusercontent.com/u/1004852?s=460&v=3" width="100px;"/><br /><sub>Michał Drzał </sub>](https://github.com/xmichaelx) <br />💻 👀 📖 💡 🤔 | [<img src="https://avatars0.githubusercontent.com/u/2261603?s=460&v=3" width="100px;"/><br /><sub>cerginio </sub>](https://github.com/cerginio) <br /> 💻 🐛 💡 🤔 | [<img src="https://avatars3.githubusercontent.com/u/13407080?s=460&v=3" width="100px;"/><br /><sub>Sven Dummis </sub>](https://github.com/svendu) <br /> 📖| [<img src="https://avatars1.githubusercontent.com/u/1687087?s=460&v=3" width="100px;"/><br /><sub>Seaear</sub>](https://github.com/Seaear) <br />🌍|
| :---: | :---: | :---: | :---: | :---: | :---: |
|[<img src="https://avatars1.githubusercontent.com/u/1150473?s=460&v=3" width="118px;"/><br /><sub>Rune Antonsen </sub>](https://github.com/ruant) <br />🐛|[<img src="https://avatars1.githubusercontent.com/u/5537607?s=460&v=3" width="118px;"/><br /><sub>Sindre Njøsen </sub>](https://github.com/Sindrenj) <br />💻|
<!-- prettier-ignore-end -->

This project follows the [all-contributors](https://github.com/kentcdodds/all-contributors) specification.
Contributions of any kind are welcome!

## Contact and Suggestion

I am happy to share my attempt of the implementation of the administration for IdentityServer4 and ASP.NET Core Identity.

Any feedback is welcome - feel free to create an issue or send me an email - [jan@skoruba.com](mailto:jan@skoruba.com). Thank you :blush:
