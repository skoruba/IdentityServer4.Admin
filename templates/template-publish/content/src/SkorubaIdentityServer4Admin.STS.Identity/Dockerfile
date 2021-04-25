FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/SkorubaIdentityServer4Admin.STS.Identity/SkorubaIdentityServer4Admin.STS.Identity.csproj", "src/SkorubaIdentityServer4Admin.STS.Identity/"]
COPY ["src/SkorubaIdentityServer4Admin.Admin.EntityFramework.Shared/SkorubaIdentityServer4Admin.Admin.EntityFramework.Shared.csproj", "src/SkorubaIdentityServer4Admin.Admin.EntityFramework.Shared/"]
COPY ["src/SkorubaIdentityServer4Admin.Shared/SkorubaIdentityServer4Admin.Shared.csproj", "src/SkorubaIdentityServer4Admin.Shared/"]
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