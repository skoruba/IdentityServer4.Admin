FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/SkorubaIdentityServer4Admin.STS.Identity/SkorubaIdentityServer4Admin.STS.Identity.csproj", "src/SkorubaIdentityServer4Admin.STS.Identity/"]
COPY ["src/SkorubaIdentityServer4Admin.Admin.EntityFramework.Shared/SkorubaIdentityServer4Admin.Admin.EntityFramework.Shared.csproj", "src/SkorubaIdentityServer4Admin.Admin.EntityFramework.Shared/"]
COPY ["src/SkorubaIdentityServer4Admin.Admin.EntityFramework.SqlServer/SkorubaIdentityServer4Admin.Admin.EntityFramework.SqlServer.csproj", "src/SkorubaIdentityServer4Admin.Admin.EntityFramework.SqlServer/"]
COPY ["src/SkorubaIdentityServer4Admin.Admin.EntityFramework.PostgreSQL/SkorubaIdentityServer4Admin.Admin.EntityFramework.PostgreSQL.csproj", "src/SkorubaIdentityServer4Admin.Admin.EntityFramework.PostgreSQL/"]
COPY ["src/SkorubaIdentityServer4Admin.Admin.EntityFramework.MySql/SkorubaIdentityServer4Admin.Admin.EntityFramework.MySql.csproj", "src/SkorubaIdentityServer4Admin.Admin.EntityFramework.MySql/"]
RUN dotnet restore "src/SkorubaIdentityServer4Admin.STS.Identity/SkorubaIdentityServer4Admin.STS.Identity.csproj"
COPY . .
WORKDIR "/src/src/SkorubaIdentityServer4Admin.STS.Identity"
RUN dotnet build "SkorubaIdentityServer4Admin.STS.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SkorubaIdentityServer4Admin.STS.Identity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "SkorubaIdentityServer4Admin.STS.Identity.dll"]