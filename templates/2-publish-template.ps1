$templateNuspecPath = "template-publish/Skoruba.IdentityServer4.Admin.Templates.nuspec"
nuget pack $templateNuspecPath

dotnet new --debug:reinit

$templateLocalName = "Skoruba.IdentityServer4.Admin.Templates.1.0.0-rc1-update2.nupkg"
dotnet.exe new -i $templateLocalName

dotnet.exe new skoruba.is4admin --name MyProject --title MyProject --adminemail 'admin@skoruba.com' --adminpassword 'Pa$$word123' --adminrole MyRole --adminclientid MyClientId --adminclientsecret MyClientSecret --dockersupport true