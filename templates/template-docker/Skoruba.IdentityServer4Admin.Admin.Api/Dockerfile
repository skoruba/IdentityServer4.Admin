FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Skoruba.IdentityServer4Admin.Admin.Api/Skoruba.IdentityServer4Admin.Admin.Api.csproj", "src/Skoruba.IdentityServer4Admin.Admin.Api/"]
COPY ["src/Skoruba.IdentityServer4Admin.Admin.EntityFramework.Shared/Skoruba.IdentityServer4Admin.Admin.EntityFramework.Shared.csproj", "src/Skoruba.IdentityServer4Admin.Admin.EntityFramework.Shared/"]
COPY ["src/Skoruba.IdentityServer4Admin.Admin.EntityFramework.SqlServer/Skoruba.IdentityServer4Admin.Admin.EntityFramework.SqlServer.csproj", "src/Skoruba.IdentityServer4Admin.Admin.EntityFramework.SqlServer/"]
COPY ["src/Skoruba.IdentityServer4Admin.Shared/Skoruba.IdentityServer4Admin.Shared.csproj", "src/Skoruba.IdentityServer4Admin.Shared/"]
COPY ["src/Skoruba.IdentityServer4Admin.Admin.EntityFramework.PostgreSQL/Skoruba.IdentityServer4Admin.Admin.EntityFramework.PostgreSQL.csproj", "src/Skoruba.IdentityServer4Admin.Admin.EntityFramework.PostgreSQL/"]
COPY ["src/Skoruba.IdentityServer4Admin.Admin.EntityFramework.MySql/Skoruba.IdentityServer4Admin.Admin.EntityFramework.MySql.csproj", "src/Skoruba.IdentityServer4Admin.Admin.EntityFramework.MySql/"]
RUN dotnet restore "src/Skoruba.IdentityServer4Admin.Admin.Api/Skoruba.IdentityServer4Admin.Admin.Api.csproj"
COPY . .
WORKDIR "/src/src/Skoruba.IdentityServer4Admin.Admin.Api"
RUN dotnet build "Skoruba.IdentityServer4Admin.Admin.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Skoruba.IdentityServer4Admin.Admin.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "Skoruba.IdentityServer4Admin.Admin.Api.dll"]