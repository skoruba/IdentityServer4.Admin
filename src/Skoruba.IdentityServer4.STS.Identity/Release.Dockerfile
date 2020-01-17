FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app
EXPOSE 80
COPY publish/ ./

ENTRYPOINT ["dotnet", "Skoruba.IdentityServer4.STS.Identity.dll"]