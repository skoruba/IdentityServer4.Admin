param([string] $migration = 'DbInit', [string] $migrationProviderName = 'All')
$currentPath = Get-Location
Set-Location ../src/Skoruba.IdentityServer4.Admin

Copy-Item appsettings.json -Destination appsettings-backup.json
$settings = Get-Content appsettings.json -raw

$dbProviders = @{ }

$dbProviders.Add("SqlServer", "..\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer\Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.csproj")
$dbProviders.Add("PostgreSQL", "..\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL\Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.csproj")
$dbProviders.Add("MySql", "..\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.MySql\Skoruba.IdentityServer4.Admin.EntityFramework.MySql.csproj")

foreach ($key in $dbProviders.Keys) {

    if ($migrationProviderName -eq 'All' -or $migrationProviderName -eq $key) {
    
        Write-Host "Generate migration for db provider:" $key ", for project path - " $dbProviders[$key]

        $providerName = '"ProviderType": "' + $key + '"'

        $settings = $settings -replace '"ProviderType".*', $providerName
        $settings | set-content appsettings.json

        dotnet ef migrations add $migration -c AdminIdentityDbContext -o Migrations/Identity -p $dbProviders[$key]
        dotnet ef migrations add $migration -c AdminLogDbContext -o Migrations/Logging -p $dbProviders[$key]
        dotnet ef migrations add $migration -c IdentityServerConfigurationDbContext -o Migrations/IdentityServerConfiguration -p $dbProviders[$key]
        dotnet ef migrations add $migration -c IdentityServerPersistedGrantDbContext -o Migrations/IdentityServerGrants -p $dbProviders[$key]
        dotnet ef migrations add $migration -c AdminAuditLogDbContext -o Migrations/AuditLogging -p $dbProviders[$key]
    }
}

Remove-Item appsettings.json
Copy-Item appsettings-backup.json -Destination appsettings.json
Remove-Item appsettings-backup.json
Set-Location $currentPath