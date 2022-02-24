param([string] $packagesVersions, [string]$gitBranchName = 'dev')

# This script contains following steps:
# - Download latest version of Skoruba.IdentityServer4.Admin from git repository
# - Use folders src and tests for project template
# - Create db migrations for seed data

$gitProject = "https://github.com/skoruba/IdentityServer4.Admin"
$gitProjectFolder = "Skoruba.IdentityServer4.Admin"
$templateSrc = "template-build/content/src"
$templateRoot = "template-build/content"
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

# Copy Docker files
Copy-Item ./$gitProjectFolder/docker-compose.dcproj $templateRoot -recurse -force
Copy-Item ./$gitProjectFolder/.dockerignore $templateRoot -recurse -force
Copy-Item ./$gitProjectFolder/docker-compose.override.yml $templateRoot -recurse -force
Copy-Item ./$gitProjectFolder/docker-compose.vs.debug.yml $templateRoot -recurse -force
Copy-Item ./$gitProjectFolder/docker-compose.vs.release.yml $templateRoot -recurse -force
Copy-Item ./$gitProjectFolder/docker-compose.yml $templateRoot -recurse -force
Copy-Item ./$gitProjectFolder/shared $templateRoot -recurse -force
Copy-Item ./$gitProjectFolder/package $templateRoot -recurse -force
Copy-Item ./$gitProjectFolder/LICENSE.md $templateRoot -recurse -force

Copy-Item ./$gitProjectFolder/Directory.Build.props $templateRoot -recurse -force

# Clean up created folders
Remove-Item ./$gitProjectFolder -recurse -force

# Clean solution and folders bin, obj
CleanBinObjFolders

# Remove references

# API
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.Api/Skoruba.IdentityServer4.Admin.Api.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic.Identity\Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.csproj
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.Api/Skoruba.IdentityServer4.Admin.Api.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic\Skoruba.IdentityServer4.Admin.BusinessLogic.csproj
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.Api/Skoruba.IdentityServer4.Admin.Api.csproj reference ..\Skoruba.IdentityServer4.Shared.Configuration\Skoruba.IdentityServer4.Shared.Configuration.csproj

# Admin
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic\Skoruba.IdentityServer4.Admin.BusinessLogic.csproj
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj reference ..\Skoruba.IdentityServer4.Admin.UI\Skoruba.IdentityServer4.Admin.UI.csproj

# STS
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj reference ..\Skoruba.IdentityServer4.Shared.Configuration\Skoruba.IdentityServer4.Shared.Configuration.csproj
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework.Configuration\Skoruba.IdentityServer4.Admin.EntityFramework.Configuration.csproj

# EF Shared
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.Shared/Skoruba.IdentityServer4.Admin.EntityFramework.Shared.csproj reference ..\Skoruba.IdentityServer4.Admin.EntityFramework.Configuration\Skoruba.IdentityServer4.Admin.EntityFramework.Configuration.csproj

# Shared
dotnet.exe remove ./$templateSrc/Skoruba.IdentityServer4.Shared/Skoruba.IdentityServer4.Shared.csproj reference ..\Skoruba.IdentityServer4.Admin.BusinessLogic.Identity\Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.csproj

# Add nuget packages
# Admin
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj package Skoruba.IdentityServer4.Admin.BusinessLogic -v $packagesVersions
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj package Skoruba.IdentityServer4.Admin.BusinessLogic.Identity -v $packagesVersions
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj package Skoruba.IdentityServer4.Admin.UI -v $packagesVersions

# STS
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj package Skoruba.IdentityServer4.Shared.Configuration -v $packagesVersions
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj package Skoruba.IdentityServer4.Admin.EntityFramework.Configuration -v $packagesVersions

# API
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin.Api/Skoruba.IdentityServer4.Admin.Api.csproj package Skoruba.IdentityServer4.Admin.BusinessLogic -v $packagesVersions
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin.Api/Skoruba.IdentityServer4.Admin.Api.csproj package Skoruba.IdentityServer4.Admin.BusinessLogic.Identity -v $packagesVersions
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin.Api/Skoruba.IdentityServer4.Admin.Api.csproj package Skoruba.IdentityServer4.Shared.Configuration -v $packagesVersions

# EF Shared
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.Shared/Skoruba.IdentityServer4.Admin.EntityFramework.Shared.csproj package Skoruba.IdentityServer4.Admin.EntityFramework.Configuration -v $packagesVersions

# Shared
dotnet.exe add ./$templateSrc/Skoruba.IdentityServer4.Shared/Skoruba.IdentityServer4.Shared.csproj package Skoruba.IdentityServer4.Admin.BusinessLogic.Identity -v $packagesVersions

# Clean solution and folders bin, obj
CleanBinObjFolders

# Clean up projects which will be installed via nuget packages
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.BusinessLogic -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.BusinessLogic.Identity -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.BusinessLogic.Shared -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.Identity -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.Extensions -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.EntityFramework.Configuration -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Shared.Configuration -Force -recurse
Remove-Item ./$templateSrc/Skoruba.IdentityServer4.Admin.UI -Force -recurse
Remove-Item ./$templateTests -Force -recurse

######################################
# Step 2
$templateNuspecPath = "template-build/Skoruba.IdentityServer4.Admin.Templates.nuspec"
nuget pack $templateNuspecPath -NoDefaultExcludes

######################################
# Step 3
$templateLocalName = "Skoruba.IdentityServer4.Admin.Templates.$packagesVersions.nupkg"

dotnet.exe new --uninstall Skoruba.IdentityServer4.Admin.Templates
dotnet.exe new -i $templateLocalName

######################################
# Step 4
# Create template for fixing project name
dotnet new skoruba.is4admin --name SkorubaIdentityServer4Admin --title "Skoruba IdentityServer4 Admin" --adminrole SkorubaIdentityAdminAdministrator --adminclientid skoruba_identity_admin --adminclientsecret skoruba_admin_client_secret

######################################
# Step 5
# Replace files

CleanBinObjFolders

$templateFiles = Get-ChildItem .\SkorubaIdentityServer4Admin\src -include *.cs, *.csproj, *.cshtml -Recurse
foreach ($file in $templateFiles) {
    Write-Host $file.PSPath


    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "SkorubaIdentityServer4Admin.Shared.Configuration", "Skoruba.IdentityServer4.Shared.Configuration" } |
    Set-Content $file.PSPath -Encoding UTF8

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "SkorubaIdentityServer4Admin.Admin.UI", "Skoruba.IdentityServer4.Admin.UI" } |
    Set-Content $file.PSPath -Encoding UTF8

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "SkorubaIdentityServer4Admin.Admin.BusinessLogic", "Skoruba.IdentityServer4.Admin.BusinessLogic" } |
    Set-Content $file.PSPath -Encoding UTF8

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "SkorubaIdentityServer4Admin.Admin.EntityFramework", "Skoruba.IdentityServer4.Admin.EntityFramework" } |
    Set-Content $file.PSPath -Encoding UTF8

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "Skoruba.IdentityServer4.Admin.EntityFramework.Shared", "SkorubaIdentityServer4Admin.Admin.EntityFramework.Shared" } |
    Set-Content $file.PSPath -Encoding UTF8

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "Skoruba.IdentityServer4.Admin.EntityFramework.MySql", "SkorubaIdentityServer4Admin.Admin.EntityFramework.MySql" } |
    Set-Content $file.PSPath -Encoding UTF8

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL", "SkorubaIdentityServer4Admin.Admin.EntityFramework.PostgreSQL" } |
    Set-Content $file.PSPath -Encoding UTF8

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer", "SkorubaIdentityServer4Admin.Admin.EntityFramework.SqlServer" } |
    Set-Content $file.PSPath -Encoding UTF8
}

CleanBinObjFolders