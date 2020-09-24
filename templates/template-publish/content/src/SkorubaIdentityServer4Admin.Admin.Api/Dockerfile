#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/SkorubaIdentityServer4Admin.Admin.Api/SkorubaIdentityServer4Admin.Admin.Api.csproj", "src/SkorubaIdentityServer4Admin.Admin.Api/"]
COPY ["src/SkorubaIdentityServer4Admin.Admin.EntityFramework.Shared/SkorubaIdentityServer4Admin.Admin.EntityFramework.Shared.csproj", "src/SkorubaIdentityServer4Admin.Admin.EntityFramework.Shared/"]
COPY ["src/SkorubaIdentityServer4Admin.Admin.EntityFramework.SqlServer/SkorubaIdentityServer4Admin.Admin.EntityFramework.SqlServer.csproj", "src/SkorubaIdentityServer4Admin.Admin.EntityFramework.SqlServer/"]
COPY ["src/SkorubaIdentityServer4Admin.Admin.EntityFramework.PostgreSQL/SkorubaIdentityServer4Admin.Admin.EntityFramework.PostgreSQL.csproj", "src/SkorubaIdentityServer4Admin.Admin.EntityFramework.PostgreSQL/"]
COPY ["src/SkorubaIdentityServer4Admin.Shared/SkorubaIdentityServer4Admin.Shared.csproj", "src/SkorubaIdentityServer4Admin.Shared/"]
COPY ["src/SkorubaIdentityServer4Admin.Admin.EntityFramework.MySql/SkorubaIdentityServer4Admin.Admin.EntityFramework.MySql.csproj", "src/SkorubaIdentityServer4Admin.Admin.EntityFramework.MySql/"]
RUN dotnet restore "src/SkorubaIdentityServer4Admin.Admin.Api/SkorubaIdentityServer4Admin.Admin.Api.csproj"
COPY . .
WORKDIR "/src/src/SkorubaIdentityServer4Admin.Admin.Api"
RUN dotnet build "SkorubaIdentityServer4Admin.Admin.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SkorubaIdentityServer4Admin.Admin.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "SkorubaIdentityServer4Admin.Admin.Api.dll"]