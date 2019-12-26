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
$packagesVersions = "1.0.0-beta8"

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

# Clean solution and folders bin, obj
CleanBinObjFolders

# Remove references

# API
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.Api/Skoruba.IdentityServer4.Admin.Api.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic.Identity\Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.csproj
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.Api/Skoruba.IdentityServer4.Admin.Api.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic.Shared\Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.csproj
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.Api/Skoruba.IdentityServer4.Admin.Api.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic\Skoruba.IdentityServer4.Admin.BusinessLogic.csproj

# Admin
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic.Identity\Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.csproj
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic.Shared\Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.csproj
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic\Skoruba.IdentityServer4.Admin.BusinessLogic.csproj

# STS
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework\Skoruba.IdentityServer4.Admin.EntityFramework.csproj
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework.Identity\Skoruba.IdentityServer4.Admin.EntityFramework.Identity.csproj

# EF Shared
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.Shared/Skoruba.IdentityServer4.Admin.EntityFramework.Shared.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework\Skoruba.IdentityServer4.Admin.EntityFramework.csproj
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.Shared/Skoruba.IdentityServer4.Admin.EntityFramework.Shared.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework.Identity\Skoruba.IdentityServer4.Admin.EntityFramework.Identity.csproj

# EF MySql
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.MySql/Skoruba.IdentityServer4.Admin.EntityFramework.MySql.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework\Skoruba.IdentityServer4.Admin.EntityFramework.csproj
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.MySql/Skoruba.IdentityServer4.Admin.EntityFramework.MySql.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework.Identity\Skoruba.IdentityServer4.Admin.EntityFramework.Identity.csproj

# EF PostgreSQL
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL/Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework\Skoruba.IdentityServer4.Admin.EntityFramework.csproj
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL/Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework.Identity\Skoruba.IdentityServer4.Admin.EntityFramework.Identity.csproj

# EF SqlServer
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer/Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework\Skoruba.IdentityServer4.Admin.EntityFramework.csproj
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer/Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework.Identity\Skoruba.IdentityServer4.Admin.EntityFramework.Identity.csproj


# Add nuget packages
# Admin
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj package Skoruba.IdentityServer4.Admin.BusinessLogic -v $packagesVersions
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj package Skoruba.IdentityServer4.Admin.BusinessLogic.Identity -v $packagesVersions

# API
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin.Api/Skoruba.IdentityServer4.Admin.Api.csproj package Skoruba.IdentityServer4.Admin.BusinessLogic -v $packagesVersions
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin.Api/Skoruba.IdentityServer4.Admin.Api.csproj package Skoruba.IdentityServer4.Admin.BusinessLogic.Identity -v $packagesVersions

# EF Shared
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.Shared/Skoruba.IdentityServer4.Admin.EntityFramework.Shared.csproj package Skoruba.IdentityServer4.Admin.EntityFramework -v $packagesVersions
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.Shared/Skoruba.IdentityServer4.Admin.EntityFramework.Shared.csproj package Skoruba.IdentityServer4.Admin.EntityFramework.Identity -v $packagesVersions

# EF MySql
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.MySql/Skoruba.IdentityServer4.Admin.EntityFramework.MySql.csproj package Skoruba.IdentityServer4.Admin.EntityFramework -v $packagesVersions
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.MySql/Skoruba.IdentityServer4.Admin.EntityFramework.MySql.csproj package Skoruba.IdentityServer4.Admin.EntityFramework.Identity -v $packagesVersions

# EF PostgreSQL
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL/Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.csproj package Skoruba.IdentityServer4.Admin.EntityFramework -v $packagesVersions
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL/Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.csproj package Skoruba.IdentityServer4.Admin.EntityFramework.Identity -v $packagesVersions


# EF SqlServer
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer/Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.csproj package Skoruba.IdentityServer4.Admin.EntityFramework -v $packagesVersions
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer/Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.csproj package Skoruba.IdentityServer4.Admin.EntityFramework.Identity -v $packagesVersions


# Clean solution and folders bin, obj
CleanBinObjFolders

# Clean up projects which will be installed via nuget packages
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.BusinessLogic -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.BusinessLogic.Identity -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.BusinessLogic.Shared -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.Identity -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.Extensions -Force -recurse
Remove-Item ./$templateTests -Force -recurse