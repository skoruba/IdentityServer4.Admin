namespace Skoruba.MultiTenant.Configuration
{
    public class MultiTenantConstants
    {
        public const string MissingTenantExceptionMessage = "A tenant is required.";

        // Optional tenant item key determining whether the tenant requires 2fa or not
        public const string RequiresTwoFactorAuthentication = nameof(RequiresTwoFactorAuthentication);

        // Optional tenant item key determining if the tenant is active or not
        public const string IsActive = nameof(IsActive);
    }
}