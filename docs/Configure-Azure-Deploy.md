# Introduction

Tutorial covers configuration of Admin for deploy on Azure.


## Create database

If you don't have publicly accessible database you will need to create one. Follow tutorials for creating databases on Azure:

- [SQL Server](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started-portal)
- [PostgreSQL](https://docs.microsoft.com/en-us/azure/postgresql/quickstart-create-server-database-portal)
- [MySQL](https://docs.microsoft.com/en-us/azure/mysql/quickstart-create-mysql-server-database-using-azure-portal)

Replace connection strings in appsettings with connection string to generated databse.

Then you can generate migrations and update the database. In `src\Skoruba.IdentityServer4.Admin` issue:

```
dotnet ef migrations add DbInit -c AdminDbContext -o Data/Migrations
dotnet ef database update -c AdminDbContext
```

## Deploying webbaps to Azure App Service

We will assume in the tutorial that STS and Admin were deployed to:

- https://is4-sts.azurewebsites.net - STS
- https://is4-admin.azurewebsites.net - Admin panel


### Updating urls

Remember to replace those values with your own in `src\Skoruba.IdentityServer4.Admin\appsettings.json` before first publish -

```json
"AdminConfiguration": {
	"IdentityAdminBaseUrl": "https://is4-admin.azurewebsites.net",
	"IdentityAdminRedirectUri": "https://is4-admin.azurewebsites.net/signin-oidc",
	"IdentityServerBaseUrl": "https://is4-sts.azurewebsites.net"
}
```

Follow instructions from: https://docs.microsoft.com/en-us/visualstudio/deployment/quickstart-deploy-to-azure


### Adding certificate for signing tokens

We also need to upload pfx certificate for signing tokens. If you don't have one here are the steps to do it:

Self-signed certificate is enough for this, we can create it using openssl (remember to write down the password - we will need it later):

```
openssl genrsa 2048 > private.pem
openssl req -x509 -new -key private.pem -out public.pem
openssl pkcs12 -export -in public.pem -inkey private.pem -out mycert.pfx
```


Now we can upload the certificate in Azure Portal to our website:

![Where to upload](Images/certificate_upload.PNG)

While we're at it we can allow only https traffic to our STS and admin:

![Always https](Images/https_always.PNG)

Last step before deploy - we need to update `src/Skoruba.IdentityServer4.STS.Identity/appsettings.json` and modify following lines:

```json
"CertificateConfiguration": {
    "UseTemporarySigningKeyForDevelopment": false,
    "UseSigningCertificateThumbprint": true,
    "SigningCertificateThumbprint": "<enter here thumbprint from Azure>",
    "UseValidationCertificateThumbprint": true,
    "ValidationCertificateThumbprint": "<enter here thumbprint from Azure>"
}
```

Now we can (re)deploy both apps to Azure.
