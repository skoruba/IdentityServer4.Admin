$packagesOutput = ".\packages"

dotnet pack .\..\src\Skoruba.IdentityServer4.Admin.BusinessLogic\Skoruba.IdentityServer4.Admin.BusinessLogic.csproj -c Release -o $packagesOutput
dotnet pack .\..\src\Skoruba.IdentityServer4.Admin.BusinessLogic.Identity\Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.csproj -c Release -o $packagesOutput
dotnet pack .\..\src\Skoruba.IdentityServer4.Admin.BusinessLogic.Shared\Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.csproj -c Release -o $packagesOutput

dotnet pack .\..\src\Skoruba.IdentityServer4.Admin.EntityFramework\Skoruba.IdentityServer4.Admin.EntityFramework.csproj -c Release -o $packagesOutput
dotnet pack .\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.Extensions\Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.csproj -c Release -o $packagesOutput
dotnet pack .\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.Identity\Skoruba.IdentityServer4.Admin.EntityFramework.Identity.csproj -c Release -o $packagesOutput
dotnet pack .\..\src\Skoruba.IdentityServer4.Admin.EntityFramework.Shared\Skoruba.IdentityServer4.Admin.EntityFramework.Shared.csproj -c Release -o $packagesOutput