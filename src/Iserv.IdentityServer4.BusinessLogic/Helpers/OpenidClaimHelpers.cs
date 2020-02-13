using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;

namespace Iserv.IdentityServer4.BusinessLogic.Helpers
{
    public static class OpenIdClaimHelpers
    {
        private readonly static string _fieldId = "id";
        private readonly static string _fieldIdExt = "idext";
        private readonly static string _fieldLogin = "login";
        private readonly static string _fieldPassword = "password";
        private readonly static string[] _fieldsEmail = new string[] { "email", "e_mail" };
        private readonly static string[] _fieldsPhone = new string[] { "phone", "phone_number" };
        private readonly static string _fieldEmailConfirmed = "emailConfirmed";
        private readonly static string _fieldPhoneConfirmed = "phoneConfirmed";

        private static void validNotNullField(Dictionary<string, object> item, string fieldName)
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

        private static object getValue(Dictionary<string, object> userFields, string[] keys)
        {
            foreach (var key in keys)
            {
                var value = userFields.GetValueOrDefault(key);
                if (value != null)
                    return value;
            }
            return null;
        }

        private static TUser GetUserBase<TUser, TKey>(Dictionary<string, object> userFields, bool isPassword, out string password)
            where TUser : UserIdentity<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            TKey id = default;
            if (userFields.ContainsKey(_fieldId))
            {
                validNotNullField(userFields, _fieldId);
                id = (TKey)Convert.ChangeType(userFields[_fieldId], typeof(TKey));
                if (id == null || id.Equals(default))
                {
                    throw new ValidationException($"Invalid field '{_fieldId}'");
                }
            }

            validNotNullField(userFields, _fieldIdExt);
            Guid idExt;
            if (!Guid.TryParse(userFields[_fieldIdExt].ToString(), out idExt))
            {
                throw new ValidationException($"Invalid field '{_fieldIdExt}'");
            }

            password = null;
            if (isPassword)
            {
                validNotNullField(userFields, _fieldPassword);
                password = userFields[_fieldPassword]?.ToString();
                if (string.IsNullOrWhiteSpace(password))
                    throw new ValidationException($"Invalid field '{_fieldPassword}'");
            }

            var email = getValue(userFields, _fieldsEmail)?.ToString();
            var phone = getValue(userFields, _fieldsPhone)?.ToString();
            
            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
                throw new ValidationException($"Fields '{string.Join(", ", _fieldsEmail)}' and '{string.Join(", ", _fieldsPhone)}' is not defined");

            var login = userFields.GetValueOrDefault(_fieldLogin)?.ToString();
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

            var emailConfirmed = userFields.GetValueOrDefault(_fieldEmailConfirmed);
            var phoneConfirmed = userFields.GetValueOrDefault(_fieldPhoneConfirmed);

            var user = new TUser
            {
                Idext = idExt,
                UserName = login,
                Email = email,
                EmailConfirmed = emailConfirmed is bool && (bool)emailConfirmed == true,
                PhoneNumber = phone,
                PhoneNumberConfirmed = phoneConfirmed is bool && (bool)phoneConfirmed == true,
            };
            if (id != null && !id.Equals(default))
            {
                user.Id = id;
            }
            return user;
        }

        public static Claim[] GetClaims<TUserKey>(Dictionary<string, object> userFields) where TUserKey : IEquatable<TUserKey>
        {
            return userFields?.Where(u => u.Value != null && u.Key != _fieldId && u.Key != _fieldIdExt
                && u.Key != _fieldLogin && u.Key != _fieldPassword
                && !_fieldsEmail.Contains(u.Key) && u.Key != _fieldEmailConfirmed
                && !_fieldsPhone.Contains(u.Key) && u.Key != _fieldPhoneConfirmed).Select(u => new Claim(u.Key, u.Value.ToString())).ToArray();
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
