FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["IdentityServer4/src/Skoruba.IdentityServer4.Admin.Api/Skoruba.IdentityServer4.Admin.Api.csproj", "IdentityServer4/src/Skoruba.IdentityServer4.Admin.Api/"]
RUN dotnet restore "IdentityServer4/src/Skoruba.IdentityServer4.Admin.Api/Skoruba.IdentityServer4.Admin.Api.csproj"
COPY . .
WORKDIR "/src/IdentityServer4/src/Skoruba.IdentityServer4.Admin.Api"
RUN dotnet build "Skoruba.IdentityServer4.Admin.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Skoruba.IdentityServer4.Admin.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Skoruba.IdentityServer4.Admin.Api.dll"]
