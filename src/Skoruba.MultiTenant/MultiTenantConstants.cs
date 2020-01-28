namespace Skoruba.MultiTenant.Configuration
{
    public class MultiTenantConstants
    {
        // TODO: Consider how to avoid having to set this...maybe make it static and set it in services extensions
        public static bool MultiTenantEnabled = true;

        public const string MissingTenantExceptionMessage = "A tenant is required.";

        // Optional tenant item key determining whether the tenant requires 2fa or not
        public const string RequiresTwoFactorAuthentication = nameof(RequiresTwoFactorAuthentication);

        // Optional tenant item key determining if the tenant is active or not
        public const string IsActive = nameof(IsActive);
    }
}