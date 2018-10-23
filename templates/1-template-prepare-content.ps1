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
$templateDataMigrationFolder = "Data/Migrations"

function CleanBinObjFolders { 

    # Clean up after migrations
    dotnet clean $templateAdminProject

    # Clean up bin, obj
    Get-ChildItem .\ -include bin, obj -Recurse | ForEach-Object ($_) { remove-item $_.fullname -Force -Recurse }    
}

# Clone the latest version from master branch
git clone $gitProject $gitProjectFolder -b $gitBranchName

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

# Add dotnet ef migrations
dotnet ef migrations add DbInit -c AdminDbContext -o $templateDataMigrationFolder -s $templateAdminProject -p $templateAdminProject

# Clean solution and folders bin, obj
CleanBinObjFolders

# Remove references
dotnet remove ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic.Identity\Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.csproj
dotnet remove ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic.Shared\Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.csproj
dotnet remove ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic\Skoruba.IdentityServer4.Admin.BusinessLogic.csproj
dotnet remove ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts\Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts.csproj

dotnet remove ./$templateSrc/Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts\Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts.csproj
dotnet remove ./$templateSrc/Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework.Identity\Skoruba.IdentityServer4.Admin.EntityFramework.Identity.csproj
dotnet remove ./$templateSrc/Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework\Skoruba.IdentityServer4.Admin.EntityFramework.csproj

# Add nuget packages
dotnet add ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj package Skoruba.IdentityServer4.Admin.BusinessLogic -v 1.0.0-beta2
dotnet add ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj package Skoruba.IdentityServer4.Admin.BusinessLogic.Identity -v 1.0.0-beta2
dotnet add ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj package Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts -v 1.0.0-beta2

dotnet add ./$templateSrc/Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj package Skoruba.IdentityServer4.Admin.EntityFramework.Identity -v 1.0.0-beta2
dotnet add ./$templateSrc/Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj package Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts -v 1.0.0-beta2

# Clean solution and folders bin, obj
CleanBinObjFolders

# Clean up projects which will be installed via nuget packages
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.BusinessLogic -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.BusinessLogic.Identity -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.BusinessLogic.Shared -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.Identity -Force -recurse
Remove-Item ./$templateTests -Force -recurse