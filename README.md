# Skoruba.IdentityServer4.Admin

### The administration of the IdentityServer4 and Asp.Net Core Identity

### Project status
- This is currently in **beta version**

- The application is written in the **Asp.Net Core MVC - using .NET Core 2.0**
    - works only with **IdentityServer4 version 2.0 and higher**

## Demo - preview

#### [https://admin.skoruba.com](https://admin.skoruba.com)

# Installation

- `git clone https://github.com/skoruba/IdentityServer4.Admin`

### Installation client libraries

- In project folder `src/Skoruba.IdentityServer4.Admin` run instalation of client libraries
- `npm install`

### Bundling and minification

- In the project is used following budling and minification - https://docs.microsoft.com/en-us/aspnet/core/client-side/bundling-and-minification?tabs=visual-studio%2Caspnetcore2x
- In the solution is used the package: `BuildBundlerMinifier` which is allow to minified on build in the Visual Studio
- For generation Css files from Sass is used - https://github.com/madskristensen/WebCompiler - this is an extension into Visual Studio 2017, that generates following configuration file: `compilerconfig.json`


### ConnectionStrings & Secrets

- Use for development the `Secret Manager` - https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?tabs=visual-studio
- Or AppSettings json files according enviroment - `appsettings.json` and  `appsettings.production.json` that is not part of repository, because it is ignored in `.gitignore`:

```json
{
    "ConnectionStrings": {
	    "AdminConnection": ""
    }
}
```

### EF Core & data access
- Run entity framework migrations - for instance from Visual Studio command line:
    - `Add-Migration DbInit -context AdminDbContext -output Data/Migrations`
    - `Update-Database -context AdminDbContext`
    - Migrations is not part of repository - ignored in `.gitignore`

- Suggestion is use some seed data for trying it:
    - Uncomment line -> `Startup.cs` -> `Configure` -> `MigrateDbContexts(app)`
    - In folder `Configuration` are stored two files with `Clients` and `Resources` -> these are initial data -> based on sample from IdentityServer4


### Authentication and authorization

- File `Constants/AuthorizationConsts.cs` -> contains configuration constants
- In the controllers is used the authorization policy -> `AuthorizationConsts.AdministrationPolicy` -> be dafault is required the role - `AuthorizationConsts.AdministrationRole`
- With default configuration is necessary configure and run instance of IdentityServer4. It is possible to use initial migartion for creating the client as it mentioned above
    - Change the specific urls and names for the IdentityServer in `Constants/AuthorizationConsts`

### Localizations - labels, messages

- All labels and messages are stored in the resources `.resx` - locatated in `/Resources`
    - Client label descriptions from - http://docs.identityserver.io/en/release/reference/client.html
    - Api Resource label descriptions from - http://docs.identityserver.io/en/release/reference/api_resource.html
    - Identity Resource label descriptions from - http://docs.identityserver.io/en/release/reference/identity_resource.html



# Overview

- The admin contains sections:

    ![Skoruba.IdentityServer4.Admin App](https://github.com/skoruba/IdentityServer4.Admin/blob/master/docs/Images/Skoruba.IdentityServer4.Admin-Solution.png?raw=true)

    ### IdentityServer4:
    
    - **Clients**
        - It is possible to defined the configuration according the client type - by default are used client types:
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
        
    - **Api Resources**
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
    
    ### Asp.Net Core Identity
    - **Users**
        - Actions: Add, Update, Delete
        - Entities:
            - User Roles
            - User Logins
            - User Claims

    - **Roles**
        - Actions: Add, Update, Delete
        - Entities:
            - Role Claims


# UI Diagram

![Skoruba.IdentityServer4.Admin Diagram](https://github.com/skoruba/IdentityServer4.Admin/blob/master/docs/Images/Skoruba.IdentityServer4.Admin-UI.png?raw=true)

# Application Diagram

![Skoruba.IdentityServer4.Admin Diagram](https://github.com/skoruba/IdentityServer4.Admin/blob/master/docs/Images/Skoruba.IdentityServer4.Admin-App-Diagram.png?raw=true)


# Plan & Vision:

- Add more unit and integration tests :blush:
- Extend administration for another protocols

# Licence

- This repository is licensed under the **MIT license**.

# Acknowledgements

- This web application is based on these projects:

- Asp.Net Core
- IdentityServer4.EntityFramework
- Asp.Net Core Identity
- XUnit
- Fluent Assertions
- Bogus
- AutoMapper
- Serilog


Thanks to [Tomáš Hübelbauer](https://github.com/TomasHubelbauer) for initial code review

Thanks to [Dominick Baier](https://github.com/leastprivilege) and [Brock Allen](https://github.com/brockallen) - creators of IdentityServer4


# Contact and suggestion

- I am happy to share my attempt of implementation the administration for IdentityServer4 and Asp.Net Core Identity
- Any feedback is welcomed - feel free to create an issue or send me mail - [jan@skoruba.com](mailto:jan@skoruba.com) - Thanks :blush:
