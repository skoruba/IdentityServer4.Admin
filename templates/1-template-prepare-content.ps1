# This script contains following steps:
# - Download latest version of Skoruba.IdentityServer4.Admin from git repository
# - Use folders src and tests for project template
# - Create db migrations for seed data

$gitProject = "https://github.com/skoruba/IdentityServer4.Admin"
$gitBranchName = "dev"
$gitProjectFolder = "Skoruba.IdentityServer4.Admin"
$templateSrc = "template-build/content/src"
$templateTests = "template-build/content/tests"
$templateAdminProject = "template-build/content/src/Skoruba.IdentityServer4.Admin"

function CleanBinObjFolders { 

    # Clean up after migrations
    dotnet.exe clean $templateAdminProject

    # Clean up bin, obj
    Get-ChildItem .\ -include bin, obj -Recurse | ForEach-Object ($_) { Remove-Item $_.fullname -Force -Recurse }    
}

# Clone the latest version from master branch
git.exe clone $gitProject $gitProjectFolder -b $gitBranchName

# Clean up src, tests folders
if ((Test-Path -Path $templateSrc)) { Remove-Item ./$templateSrc -recurse -force }
if ((Test-Path -Path $templateTests)) { Remove-Item ./$templateTests -recurse -force }

# Create src, tests folders
if (!(Test-Path -Path $templateSrc)) { mkdir $templateSrc }
if (!(Test-Path -Path $templateTests)) { mkdir $templateTests }

# Copy the latest src and tests to content
Copy-Item ./$gitProjectFolder/src/* $templateSrc -recurse -force
Copy-Item ./$gitProjectFolder/tests/* $templateTests -recurse -force

# Clean up created folders
Remove-Item ./$gitProjectFolder -recurse -force

# Add information about adding the ef migrations
"Adding ef migrations"; 
"This process may take a few minutes, please wait...";

$templateDataMigrationFolder = "Data/Migrations"

# Add dotnet ef migrations
dotnet.exe ef migrations add AspNetIdentityDbInit -c AdminIdentityDbContext -o $templateDataMigrationFolder/Identity -s $templateAdminProject -p $templateAdminProject
dotnet.exe ef migrations add LoggingDbInit -c AdminLogDbContext -o $templateDataMigrationFolder/Logging -s $templateAdminProject -p $templateAdminProject
dotnet.exe ef migrations add IdentityServerConfigurationDbInit -c IdentityServerConfigurationDbContext -o $templateDataMigrationFolder/IdentityServerConfiguration -s $templateAdminProject -p $templateAdminProject
dotnet.exe ef migrations add IdentityServerPersistedGrantsDbInit -c IdentityServerPersistedGrantDbContext -o $templateDataMigrationFolder/IdentityServerGrants -s $templateAdminProject -p $templateAdminProject


# Clean solution and folders bin, obj
CleanBinObjFolders

# Remove references
# Admin
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic.Identity\Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.csproj
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic.Shared\Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.csproj
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic\Skoruba.IdentityServer4.Admin.BusinessLogic.csproj

# STS
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework\Skoruba.IdentityServer4.Admin.EntityFramework.csproj

# Dbcontexts
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts/Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework\Skoruba.IdentityServer4.Admin.EntityFramework.csproj

# Add nuget packages
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj package Skoruba.IdentityServer4.Admin.BusinessLogic -v 1.0.0-beta6
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj package Skoruba.IdentityServer4.Admin.BusinessLogic.Identity -v 1.0.0-beta6

dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj package Skoruba.IdentityServer4.Admin.EntityFramework -v 1.0.0-beta6

dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts/Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts.csproj package Skoruba.IdentityServer4.Admin.EntityFramework -v 1.0.0-beta6

# Clean solution and folders bin, obj
CleanBinObjFolders

# Clean up projects which will be installed via nuget packages
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.BusinessLogic -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.BusinessLogic.Identity -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.BusinessLogic.Shared -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework -Force -recurse
Remove-Item ./$templateTests -Force -recurse