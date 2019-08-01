FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Skoruba.IdentityServer4.Admin.Api/Skoruba.IdentityServer4.Admin.Api.csproj src/Skoruba.IdentityServer4.Admin.Api/
RUN dotnet restore src/Skoruba.IdentityServer4.Admin.Api/Skoruba.IdentityServer4.Admin.Api.csproj
COPY . .
WORKDIR /src/src/Skoruba.IdentityServer4.Admin.Api
RUN dotnet build Skoruba.IdentityServer4.Admin.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Skoruba.IdentityServer4.Admin.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Skoruba.IdentityServer4.Admin.Api.dll"]
