param([string] $packagesVersions)

$templateNuspecPath = "template-publish/Skoruba.IdentityServer4.Admin.Templates.nuspec"
nuget pack $templateNuspecPath -NoDefaultExcludes

dotnet.exe new --uninstall Skoruba.IdentityServer4.Admin.Templates

$templateLocalName = "Skoruba.IdentityServer4.Admin.Templates.$packagesVersions.nupkg"
dotnet.exe new -i $templateLocalName

dotnet.exe new skoruba.is4admin --name MyProject --title MyProject --adminemail 'admin@skoruba.com' --adminpassword 'Pa$$word123' --adminrole MyRole --adminclientid MyClientId --adminclientsecret MyClientSecret --dockersupport true