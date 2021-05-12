param([string] $version, [string] $key)

dotnet nuget push ../templates/Skoruba.IdentityServer4.Admin.Templates.$version.nupkg -k $key -s https://api.nuget.org/v3/index.json