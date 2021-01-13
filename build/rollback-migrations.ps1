param([string] $migration = 'DbInit', [string] $migrationProviderName = 'All', [string] $targetContext = 'All')
$projectName = "Skoruba.IdentityServer4";
$currentPath = Get-Location
Set-Location "../src/$projectName.Admin"
Copy-Item appsettings.json -Destination appsettings-backup.json
$settings = Get-Content appsettings.json -raw

#Initialze db context and define the target directory
$targetContexts = @{ 
    AdminIdentityDbContext                = "Migrations\Identity"
    AdminLogDbContext                     = "Migrations\Logging";
    IdentityServerConfigurationDbContext  = "Migrations\IdentityServerConfiguration";
    IdentityServerPersistedGrantDbContext = "Migrations\IdentityServerGrants";
    AdminAuditLogDbContext                = "Migrations\AuditLogging";
    IdentityServerDataProtectionDbContext = "Migrations\DataProtection";
}

#Initialize the db providers and it's respective projects
$dpProviders = @{
    SqlServer  = "..\..\src\$projectName.Admin.EntityFramework.SqlServer\$projectName.Admin.EntityFramework.SqlServer.csproj";
    PostgreSQL = "..\..\src\$projectName.Admin.EntityFramework.PostgreSQL\$projectName.Admin.EntityFramework.PostgreSQL.csproj";
    MySql      = "..\..\src\$projectName.Admin.EntityFramework.MySql\$projectName.Admin.EntityFramework.MySql.csproj";
}

#Fix issue when the tools is not installed and the nuget package does not work see https://github.com/MicrosoftDocs/azure-docs/issues/40048
Write-Host "Updating donet ef tools"
$env:Path += "	% USERPROFILE % \.dotnet\tools";
dotnet tool update --global dotnet-ef

Write-Host "Start migrate projects"
foreach ($provider in $dpProviders.Keys) {

    if ($migrationProviderName -eq 'All' -or $migrationProviderName -eq $provider) {
    
        $projectPath = (Get-Item -Path $dpProviders[$provider] -Verbose).FullName;
        Write-Host "Generate migration for db provider:" $provider ", for project path - " $projectPath

        $providerName = '"ProviderType": "' + $provider + '"'

        $settings = $settings -replace '"ProviderType".*', $providerName
        $settings | set-content appsettings.json
        if ((Test-Path $projectPath) -eq $true) {
            foreach ($context in $targetContexts.Keys) {
                
                if ($targetContext -eq 'All' -or $context -eq $targetContext) {

                    Write-Host "Migrating context " $context
                    dotnet ef database update $migration -c $context -p $projectPath
                }
            } 
        }
        
    }
}

Remove-Item appsettings.json
Copy-Item appsettings-backup.json -Destination appsettings.json
Remove-Item appsettings-backup.json
Set-Location $currentPath
