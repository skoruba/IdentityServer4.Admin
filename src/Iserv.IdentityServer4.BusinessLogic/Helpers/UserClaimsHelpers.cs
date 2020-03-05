using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using IdentityServer4.Extensions;

namespace Iserv.IdentityServer4.BusinessLogic.Helpers
{
    public static class UserClaimsHelpers
    {
        private const string FieldId = "id";
        private const string FieldIdExt = "idext";
        private const string FieldLogin = "login";
        private const string FieldPassword = "password";
        private static readonly string[] FieldsEmail = new string[] {"email", "e_mail"};
        private static readonly string[] FieldsPhone = new string[] {"phone", "phone_number"};
        private const string FieldEmailConfirmed = "emailConfirmed";
        private const string FieldPhoneConfirmed = "phoneConfirmed";

        private static void ValidNotNullField(Dictionary<string, object> item, string fieldName)
        {
            if (!item.ContainsKey(fieldName))
                throw new ValidationException($"Field '{fieldName}' is not defined");
        }

        public static TUser GetUserBase<TUser, TKey>(Dictionary<string, object> userFields)
            where TUser : UserIdentity<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            return GetUserBase<TUser, TKey>(userFields, false, out _);
        }

        public static TUser GetUserBase<TUser, TKey>(Dictionary<string, object> userFields, out string password)
            where TUser : UserIdentity<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            return GetUserBase<TUser, TKey>(userFields, true, out password);
        }

        private static object GetValue(IReadOnlyDictionary<string, object> userFields, string[] keys)
        {
            return keys.Select(key => userFields.GetValueOrDefault(key)).FirstOrDefault(value => value != null);
        }

        public static Guid GetIdext(IEnumerable<Claim> claims)
        {
            var val = claims.FirstOrDefault(c => c.Type == FieldIdExt)?.Value;
            if (string.IsNullOrWhiteSpace(val))
                throw new ValidationException($"Invalid field '{FieldIdExt}'");
            if (!Guid.TryParse(val, out var idext))
                throw new ValidationException($"Invalid field '{FieldIdExt}'");
            return idext;
        }

        public static Guid GetIdext(IIdentity identity)
        {
            return GetIdext(identity.GetAuthenticationMethods());
        }

        public static Guid GetIdext(Dictionary<string, object> userFields)
        {
            if (!Guid.TryParse(userFields[FieldIdExt].ToString(), out var idext))
                throw new ValidationException($"Invalid field '{FieldIdExt}'");
            return idext;
        }

        public static string GetEmail(Dictionary<string, object> userFields)
        {
            return GetValue(userFields, FieldsEmail)?.ToString();
        }

        public static string GetPhone(Dictionary<string, object> userFields)
        {
            return GetValue(userFields, FieldsPhone)?.ToString();
        }

        private static TUser GetUserBase<TUser, TKey>(Dictionary<string, object> userFields, bool isPassword, out string password)
            where TUser : UserIdentity<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            TKey id = default;
            if (userFields.ContainsKey(FieldId))
            {
                ValidNotNullField(userFields, FieldId);
                id = (TKey) Convert.ChangeType(userFields[FieldId], typeof(TKey));
                if (id == null || id.Equals(default))
                {
                    throw new ValidationException($"Invalid field '{FieldId}'");
                }
            }

            ValidNotNullField(userFields, FieldIdExt);
            if (!Guid.TryParse(userFields[FieldIdExt].ToString(), out var idext))
            {
                throw new ValidationException($"Invalid field '{FieldIdExt}'");
            }

            password = null;
            if (isPassword)
            {
                ValidNotNullField(userFields, FieldPassword);
                password = userFields[FieldPassword]?.ToString();
                if (string.IsNullOrWhiteSpace(password))
                    throw new ValidationException($"Invalid field '{FieldPassword}'");
            }

            var email = GetEmail(userFields);
            var phone = GetPhone(userFields);

            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
                throw new ValidationException($"Fields '{string.Join(", ", FieldsEmail)}' and '{string.Join(", ", FieldsPhone)}' is not defined");

            var login = userFields.GetValueOrDefault(FieldLogin)?.ToString();
            if (string.IsNullOrWhiteSpace(login))
            {
                login = string.IsNullOrWhiteSpace(email) ? phone : email;
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var vEmail = new EmailAddressAttribute();
                if (!vEmail.IsValid(email))
                    throw new ValidationException($"Invalid email. {vEmail.ErrorMessage}");
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var vPhone = new PhoneAttribute();
                if (!vPhone.IsValid(phone))
                    throw new ValidationException($"Invalid phone. {vPhone.ErrorMessage}");
            }

            var emailConfirmed = userFields.GetValueOrDefault(FieldEmailConfirmed);
            var phoneConfirmed = userFields.GetValueOrDefault(FieldPhoneConfirmed);

            var user = new TUser
            {
                Idext = idext,
                UserName = login,
                Email = email,
                EmailConfirmed = emailConfirmed is bool && (bool) emailConfirmed == true,
                PhoneNumber = phone,
                PhoneNumberConfirmed = phoneConfirmed is bool && (bool) phoneConfirmed == true,
            };
            if (id != null && !id.Equals(default))
            {
                user.Id = id;
            }

            return user;
        }

        public static Claim[] GetClaims<TUserKey>(Dictionary<string, object> userFields) where TUserKey : IEquatable<TUserKey>
        {
            return userFields?.Where(u => u.Value != null && u.Key != FieldId && u.Key != FieldIdExt
                                          && u.Key != FieldLogin && u.Key != FieldPassword
                                          && !FieldsEmail.Contains(u.Key) && u.Key != FieldEmailConfirmed
                                          && !FieldsPhone.Contains(u.Key) && u.Key != FieldPhoneConfirmed)
                .Select(u => new Claim(u.Key, u.Value.ToString() == "False" ? "false" : u.Value.ToString() == "True" ? "true" : u.Value.ToString()))
                .ToArray();
        }

        public static Dictionary<string, object> GetClaimsValues<TUserKey>(Dictionary<string, object> userFields) where TUserKey : IEquatable<TUserKey>
        {
            return userFields?.Where(u => u.Value != null && u.Key != FieldId && u.Key != FieldIdExt
                                          && u.Key != FieldLogin && u.Key != FieldPassword
                                          && !FieldsEmail.Contains(u.Key) && u.Key != FieldEmailConfirmed
                                          && !FieldsPhone.Contains(u.Key) && u.Key != FieldPhoneConfirmed).ToDictionary(i => i.Key, i => i.Value);
        }

        public static IList<Claim> ExtractClaimsToRemove(Claim[] claimsOld, Claim[] claimsNew)
        {
            var claimsToRemove = new List<Claim>();
            foreach (var claimOld in claimsOld)
            {
                var claimNew = claimsNew?.FirstOrDefault(c => c.Type == claimOld.Type);
                if (claimNew == null || string.IsNullOrWhiteSpace(claimNew.Value))
                {
                    claimsToRemove.Add(claimOld);
                }
            }

            return claimsToRemove;
        }

        public static IList<Claim> ExtractClaimsToAdd(Claim[] claimsOld, Claim[] claimsNew)
        {
            var claimsToAdd = new List<Claim>();
            foreach (var claimNew in claimsNew)
            {
                if (string.IsNullOrWhiteSpace(claimNew.Value))
                {
                    continue;
                }

                var claimOld = claimsOld?.FirstOrDefault(c => c.Type == claimNew.Type);
                if (claimOld == null || string.IsNullOrWhiteSpace(claimOld.Value))
                {
                    claimsToAdd.Add(claimNew);
                }
            }

            return claimsToAdd;
        }

        public static IList<Tuple<Claim, Claim>> ExtractClaimsToReplace(Claim[] claimsOld, Claim[] claimsNew)
        {
            var claimsToReplace = new List<Tuple<Claim, Claim>>();
            foreach (var claimOld in claimsOld)
            {
                var claimNew = claimsNew?.FirstOrDefault(c => c.Type == claimOld.Type);
                if (claimNew != null && !string.IsNullOrWhiteSpace(claimNew.Value))
                {
                    claimsToReplace.Add(new Tuple<Claim, Claim>(claimOld, claimNew));
                }
            }

            return claimsToReplace;
        }
    }
}