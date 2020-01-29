# Introduction

Tutorial covers configuration of Admin on Ubuntu 19.04 with fresh instance of PostgreSQL database.

# Prerequisites

## .NET Core 3.1 SDK

Instructions from: https://docs.microsoft.com/en-us/dotnet/core/install/linux-package-manager-ubuntu-1904

```
wget -q https://packages.microsoft.com/config/ubuntu/19.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

sudo apt-get update
sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-sdk-3.1
```

## PostgreSQL

Instructions from: https://linuxize.com/post/how-to-install-postgresql-on-ubuntu-18-04/

```
sudo apt update
sudo apt install postgresql postgresql-contrib
```

Throughout tutorial we will use PostgreSQL running on localhost and default port 5432 with username/password combination postgres/postgres. You can update connection strings with your own or if you want to follow everything exactly then after installing PostgreSQL you will need to change password of `postgres` user:

```
sudo -u postgres psql
ALTER USER postgre WITH PASSWORD 'postgres';
```

## IDE

Console commands will be used throghout the tutorial but for code editing it is recommended to use dedicated IDE. I've got good experience with [Visual Studio Code](https://code.visualstudio.com/) and [Rider](https://www.jetbrains.com/rider/).

# Cloning Admin

```
git clone https://github.com/skoruba/IdentityServer4.Admin
```

# Adjustments for PostgreSQL

By default everything is configured for Microsoft SQL Server, but fortunately it's pretty easy to change.

## Replace connection strings
First change connection strings in `src/Skoruba.IdentityServer4.Admin/appsettings.json` and  `src/Skoruba.IdentityServer4.STS.Identity/appsettings.json` and replace them with following connection string:

```
Server=localhost; User Id=postgres; Database=is4admin; Port=5432; Password=postgres; SSL Mode=Prefer; Trust Server Certificate=true
```

## Swith to PostgreSQL

It is possible to change database provider in `appsettings.json` in following projects:
 - Skoruba.IdentityServer4.Admin
 - Skoruba.IdentityServer4.Admin.Api
 - Skoruba.IdentityServer4.STS.Identity
 
Change parameter `ProviderType` in section to:
```
"DatabaseProviderConfiguration": {
    "ProviderType": "PostgreSQL"
  }
```

# Final setup 

## Run STS and Admin

First run STS in `src/Skoruba.IdentityServer4.STS.Identity` launch:

```
dotnet run
```

Admin also needs to seed the database so seperate terminal in `src/Skoruba.IdentityServer4.Admin` we add additional seed parameter:

```
dotnet run /seed
```

After that we should have STS listening on http://localhost:5000 and Admin on http://localhost:9000.  We can go to the latter and we should be redirected to our STS for authentication. You can find the default credentials in `identitydata.json` in project called `Skoruba.IdentityServer4.Admin`.

# Final thoughts

There are many more steps required before IS4 and Admin panel are sufficiently hardened to be used in production scenario. Please bear in mind that this tutorial serves only as a quickstart.
