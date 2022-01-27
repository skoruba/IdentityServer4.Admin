$templateSrc = "template-publish/content/src"
$temporaryProjectFolder = "Skoruba.IdentityServer4Admin"
$templateDockerFolder = "template-docker"

# Remove original src folder for publish folder
if ((Test-Path -Path $templateSrc)) { Remove-Item ./$templateSrc -recurse -force }

# Copy new src folder
Copy-Item ./$temporaryProjectFolder/src ./$templateSrc -recurse -force

# Copy docker files for Admin, Api and STS
Copy-Item ./$templateDockerFolder/Skoruba.IdentityServer4Admin.Admin/* $templateSrc/Skoruba.IdentityServer4Admin.Admin -recurse -force
Copy-Item ./$templateDockerFolder/Skoruba.IdentityServer4Admin.Admin.Api/* $templateSrc/Skoruba.IdentityServer4Admin.Admin.Api -recurse -force
Copy-Item ./$templateDockerFolder/Skoruba.IdentityServer4Admin.STS.Identity/* $templateSrc/Skoruba.IdentityServer4Admin.STS.Identity -recurse -force