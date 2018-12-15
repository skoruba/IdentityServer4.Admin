FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj src/Skoruba.IdentityServer4.Admin/
COPY src/Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts/Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts.csproj src/Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts/
COPY src/Skoruba.IdentityServer4.Admin.EntityFramework.Identity/Skoruba.IdentityServer4.Admin.EntityFramework.Identity.csproj src/Skoruba.IdentityServer4.Admin.EntityFramework.Identity/
COPY src/Skoruba.IdentityServer4.Admin.EntityFramework/Skoruba.IdentityServer4.Admin.EntityFramework.csproj src/Skoruba.IdentityServer4.Admin.EntityFramework/
COPY src/Skoruba.IdentityServer4.Admin.BusinessLogic/Skoruba.IdentityServer4.Admin.BusinessLogic.csproj src/Skoruba.IdentityServer4.Admin.BusinessLogic/
COPY src/Skoruba.IdentityServer4.Admin.BusinessLogic.Shared/Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.csproj src/Skoruba.IdentityServer4.Admin.BusinessLogic.Shared/
COPY src/Skoruba.IdentityServer4.Admin.BusinessLogic.Identity/Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.csproj src/Skoruba.IdentityServer4.Admin.BusinessLogic.Identity/
RUN dotnet restore src/Skoruba.IdentityServer4.Admin/Skoruba.IdentityServer4.Admin.csproj
COPY . .
WORKDIR /src/src/Skoruba.IdentityServer4.Admin
RUN dotnet build Skoruba.IdentityServer4.Admin.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Skoruba.IdentityServer4.Admin.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Skoruba.IdentityServer4.Admin.dll"]
