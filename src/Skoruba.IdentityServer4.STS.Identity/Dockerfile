FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY ["BaseService/Core.BaseDataAccessInterfaces/bin/Release/netstandard2.0/Core.BaseDataAccessInterfaces.dll", "BaseService/Core.BaseDataAccessInterfaces/bin/Release/netstandard2.0/"]
COPY ["BaseService/Core.Interfaces/bin/Release/netstandard2.0/Core.Interfaces.dll", "BaseService/Core.Interfaces/bin/Release/netstandard2.0/"]
COPY ["BaseService/Infrastucture.AutoMapper/bin/Release/netstandard2.0/Infrastucture.AutoMapper.dll", "BaseService/Infrastucture.AutoMapper/bin/Release/netstandard2.0/"]
COPY ["BaseService/Infrastucture.Newtonsoft/bin/Release/netstandard2.0/Infrastucture.Newtonsoft.dll", "BaseService/Infrastucture.Newtonsoft/bin/Release/netstandard2.0/"]
COPY ["BaseService/Infrastucture.Tools/bin/Release/netstandard2.0/Infrastucture.Tools.dll", "BaseService/Infrastucture.Tools/bin/Release/netstandard2.0/"]
COPY ["BaseService/PostrgresLink2db.BaseDataAcess/bin/Release/netstandard2.0/PostrgresLink2db.BaseDataAccess.dll", "BaseService/PostrgresLink2db.BaseDataAcess/bin/Release/netstandard2.0/"]

COPY ["IdentityServer4/src/Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj", "IdentityServer4/src/Skoruba.IdentityServer4.STS.Identity/"]
COPY ["IdentityServer4/src/Skoruba.IdentityServer4.Admin.EntityFramework.MySql/Skoruba.IdentityServer4.Admin.EntityFramework.MySql.csproj", "IdentityServer4/src/Skoruba.IdentityServer4.Admin.EntityFramework.MySql/"]
COPY ["IdentityServer4/src/Skoruba.IdentityServer4.Admin.EntityFramework.Identity/Skoruba.IdentityServer4.Admin.EntityFramework.Identity.csproj", "IdentityServer4/src/Skoruba.IdentityServer4.Admin.EntityFramework.Identity/"]
COPY ["IdentityServer4/src/Skoruba.IdentityServer4.Admin.EntityFramework/Skoruba.IdentityServer4.Admin.EntityFramework.csproj", "IdentityServer4/src/Skoruba.IdentityServer4.Admin.EntityFramework/"]
COPY ["IdentityServer4/src/Skoruba.IdentityServer4.Admin.EntityFramework.Extensions/Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.csproj", "IdentityServer4/src/Skoruba.IdentityServer4.Admin.EntityFramework.Extensions/"]
COPY ["IdentityServer4/src/Skoruba.IdentityServer4.Admin.EntityFramework.Shared/Skoruba.IdentityServer4.Admin.EntityFramework.Shared.csproj", "IdentityServer4/src/Skoruba.IdentityServer4.Admin.EntityFramework.Shared/"]
COPY ["IdentityServer4/src/Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL/Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL.csproj", "IdentityServer4/src/Skoruba.IdentityServer4.Admin.EntityFramework.PostgreSQL/"]
COPY ["IdentityServer4/src/Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer/Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.csproj", "IdentityServer4/src/Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer/"]
RUN dotnet restore "IdentityServer4/src/Skoruba.IdentityServer4.STS.Identity/Skoruba.IdentityServer4.STS.Identity.csproj"
COPY . .
WORKDIR "/src/IdentityServer4/src/Skoruba.IdentityServer4.STS.Identity"
RUN dotnet build "Skoruba.IdentityServer4.STS.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Skoruba.IdentityServer4.STS.Identity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "Skoruba.IdentityServer4.STS.Identity.dll"]