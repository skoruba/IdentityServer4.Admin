FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj", "Skoruba.IdentityServer4.STS.Identity/"]
COPY ["Skoruba.IdentityServer4.Admin.EntityFramework.Identity/Skoruba.IdentityServer4.Admin.EntityFramework.Identity.csproj", "Skoruba.IdentityServer4.Admin.EntityFramework.Identity/"]
COPY ["Skoruba.IdentityServer4.Admin.EntityFramework/Skoruba.IdentityServer4.Admin.EntityFramework.csproj", "Skoruba.IdentityServer4.Admin.EntityFramework/"]
COPY ["Skoruba.IdentityServer4.Admin.EntityFramework.Extensions/Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.csproj", "Skoruba.IdentityServer4.Admin.EntityFramework.Extensions/"]
COPY ["Skoruba.IdentityServer4.Admin.EntityFramework.Shared/Skoruba.IdentityServer4.Admin.EntityFramework.Shared.csproj", "Skoruba.IdentityServer4.Admin.EntityFramework.Shared/"]
RUN dotnet restore "Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj"
COPY . .
WORKDIR "Skoruba.IdentityServer4.STS.Identity"
RUN dotnet build "Skoruba.IdentityServer4.STS.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Skoruba.IdentityServer4.STS.Identity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Skoruba.IdentityServer4.STS.Identity.dll"]