﻿using Skoruba.IdentityServer4.Shared.Configuration.Configuration.Identity;

namespace SkorubaIdentityServer4Admin.STS.Identity.Helpers.Localization
{
    public static class LoginPolicyResolutionLocalizer
    {
        public static string GetUserNameLocalizationKey(LoginResolutionPolicy policy)
        {
            switch (policy)
            {
                case LoginResolutionPolicy.Username:
                    return "Username";
                case LoginResolutionPolicy.Email:
                    return "Email";
                default:
                    return "Username";
            }
        }
    }
}








