using Finbuckle.MultiTenant;
using System;

namespace Skoruba.MultiTenant.Finbuckle.Extensions
{
    public static class TenantInfoExtensions
    {
        #region RequiresTwoFactorAuthentication
        public const string RequiresTwoFactorAuthentication = nameof(RequiresTwoFactorAuthentication);
        public static bool? GetRequiresTwoFactorAuthentication(this TenantInfo tenantInfo)
        {
            return tenantInfo.Items.TryGetValue(nameof(RequiresTwoFactorAuthentication), out var requires2FA) ? (bool?)requires2FA : null;
        }
        public static void SetRequiresTwoFactorAuthentication(this TenantInfo tenantInfo, bool? value)
        {
            if (tenantInfo.Items.ContainsKey(RequiresTwoFactorAuthentication))
            {
                if (value.HasValue)
                {
                    tenantInfo.Items[RequiresTwoFactorAuthentication] = value;
                }
                else
                {
                    tenantInfo.Items.Remove(RequiresTwoFactorAuthentication);
                }
            }
            else
            {
                if (value.HasValue)
                {
                    tenantInfo.Items.Add(RequiresTwoFactorAuthentication, value);
                }
            }
        }

        #endregion

        #region IsActive
        public const string IsActive = nameof(IsActive);
        public static bool? GetIsActive(this TenantInfo tenantInfo)
        {
            return tenantInfo.Items.TryGetValue(nameof(IsActive), out var isActive) ? (bool?)isActive : null;
        }
        public static void SetIsActive(this TenantInfo tenantInfo, bool? value)
        {
            if (tenantInfo.Items.ContainsKey(IsActive))
            {
                if (value.HasValue)
                {
                    tenantInfo.Items[IsActive] = value;
                }
                else
                {
                    tenantInfo.Items.Remove(IsActive);
                }
            }
            else
            {
                if (value.HasValue)
                {
                    tenantInfo.Items.Add(IsActive, value);
                }
            }
        }

        #endregion

        //public const string Code = nameof(Code);


        //public static string GetCode(this TenantInfo tenantInfo)
        //{
        //    return tenantInfo.Items.TryGetValue(nameof(Code), out var code) ? (string)code : null;
        //}


        //public static void SetCode(this TenantInfo tenantInfo, string value)
        //{
        //    if (tenantInfo.Items.ContainsKey(Code))
        //    {
        //        if (!string.IsNullOrWhiteSpace(value))
        //        {
        //            tenantInfo.Items[Code] = value;
        //        }
        //        else
        //        {
        //            tenantInfo.Items.Remove(Code);
        //        }
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrWhiteSpace(value))
        //        {
        //            tenantInfo.Items.Add(Code, value);
        //        }
        //    }
        //}


    }
}