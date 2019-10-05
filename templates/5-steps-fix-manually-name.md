# Fix namespaces for nuget package:

# Replace: 
SkorubaIdentityServer4Admin.Admin.BusinessLogic
with:
Skoruba.IdentityServer4.Admin.BusinessLogic

# Replace:
SkorubaIdentityServer4Admin.Admin.EntityFramework
with:
Skoruba.IdentityServer4.Admin.EntityFramework

# Replace:
Skoruba.IdentityServer4.Admin.EntityFramework.Shared
with:
SkorubaIdentityServer4Admin.Admin.EntityFramework.Shared

# Fix nuget package name
# Copy fixed src from SkorubaIdentityServer4Admin to template-publish

# Add note for RequireHttpsMetadata set for true for production usage