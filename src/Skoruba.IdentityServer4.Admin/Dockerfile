FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj src/Skoruba.IdentityServer4.Admin/
RUN dotnet restore src/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj
COPY . .
WORKDIR /src/src/Skoruba.IdentityServer4.Admin
RUN dotnet ef migrations add AspNetIdentityDbInit -c AdminIdentityDbContext -o Data/Migrations/Identity
RUN dotnet ef migrations add LoggingDbInit  -c AdminLogDbContext  -o Data/Migrations/Logging
RUN dotnet ef migrations add IdentityServerConfigurationDbInit  -c IdentityServerConfigurationDbContext -o Data/Migrations/IdentityServerConfiguration
RUN dotnet ef migrations add IdentityServerPersistedGrantsDbInit  -c IdentityServerPersistedGrantDbContext -o Data/Migrations/IdentityServerGrants
RUN dotnet build Skoruba.IdentityServer4.Admin.csproj -c Debug -o /app

FROM build AS publish
RUN dotnet publish Skoruba.IdentityServer4.Admin.csproj -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Skoruba.IdentityServer4.Admin.dll"]