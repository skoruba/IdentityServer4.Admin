namespace Skoruba.IdentityServer4.STS.Identity.Configuration.Constants
{
    public class MobileAuthConstants
    {
        public struct TokenRequest
        {
            public const string PhoneNumber = "phone_number";
            public const string VerificationToken = "verification_token";
            public const string ProtectToken = "protect_token";
        }


        public struct TokenPurpose
        {
            public const string MobilePasswordAuth = "mobile_password-auth";
        }

        public struct GrantType
        {
            public const string PhoneNumberToken = "phone_number";
        }

        public struct SecretKeys
        {
            public const string PhoneNumberSecretKey = "abcdefghijklmnopqrstuvwxyz0123456789";
        }
    }
}