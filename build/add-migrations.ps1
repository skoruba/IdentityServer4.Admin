param([string] $migrations = 'DbInit')
$currentPath = Get-Location
Set-Location ../src/Skoruba.IdentityServer4.Admin

Copy-Item appsettings.json -Destination appsettings-backup.json
$settings = Get-Content appsettings.json -raw

# SQL Server Migration
$settings = $settings -replace '"ProviderType".*', '"ProviderType": "SqlServer",'
$settings | set-content appsettings.json

dotnet ef migrations add $migrations -c AdminIdentityDbContext -o Migrations/Identity -p ..\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer\Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.csproj
dotnet ef migrations add $migrations -c AdminLogDbContext -o Migrations/Logging -p ..\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer\Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.csproj
dotnet ef migrations add $migrations -c IdentityServerConfigurationDbContext -o Migrations/IdentityServerConfiguration -p ..\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer\Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.csproj
dotnet ef migrations add $migrations -c IdentityServerPersistedGrantDbContext -o Migrations/IdentityServerGrants -p ..\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer\Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.csproj
dotnet ef migrations add $migrations -c AdminAuditLogDbContext -o Migrations/AuditLogging -p ..\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer\Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.csproj

# Postgre Migration
$settings = $settings -replace '"ProviderType".*', '"ProviderType": "PostgreSQL"'
$settings | set-content appsettings.json

dotnet ef migrations add $migrations -c AdminIdentityDbContext -o Migrations/Identity -p ..\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL\Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.csproj
dotnet ef migrations add $migrations -c AdminLogDbContext -o Migrations/Logging -p ..\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL\Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.csproj
dotnet ef migrations add $migrations -c IdentityServerConfigurationDbContext -o Migrations/IdentityServerConfiguration -p ..\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL\Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.csproj
dotnet ef migrations add $migrations -c IdentityServerPersistedGrantDbContext -o Migrations/IdentityServerGrants -p ..\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL\Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.csproj
dotnet ef migrations add $migrations -c AdminAuditLogDbContext -o Migrations/AuditLogging -p ..\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL\Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.csproj

Remove-Item appsettings.json
Copy-Item appsettings-backup.json -Destination appsettings.json
Remove-Item appsettings-backup.json
Set-Location $currentPath