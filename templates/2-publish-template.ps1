$templateNuspecPath = "template-publish/Skoruba.IdentityServer4.Admin.Templates.nuspec"
nuget pack $templateNuspecPath

$templateLocalName = "Skoruba.IdentityServer4.Admin.Templates.1.0.0-beta8.nupkg"
dotnet.exe new -i $templateLocalName

dotnet.exe new skoruba.is4admin --name MyProject --title MyProject --adminemail admin@skoruba.com --adminpassword Pa$$word123 --adminrole MyRole --adminclientid MyClientId --adminclientsecret MyClientSecret --dockersupport true