using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Helpers
{
    public static class OpenIdClaimHelpers
    {
        private readonly static string _fieldId = "id";
        private readonly static string _fieldIdExt = "idext";
        private readonly static string _fieldLogin = "login";
        private readonly static string _fieldPassword = "password";
        private readonly static string _fieldEmail = "e_mail";
        private readonly static string _fieldPhone = "phone_number";

        private static void validNotNullField(Dictionary<string, object> item, string fieldName)
        {
            if (!item.ContainsKey(fieldName))
                throw new ValidationException($"Field '{fieldName}' is not defined");
        }

        public static TUser GetUserBase<TUser, TKey>(Dictionary<string, object> userFields, out string password)
            where TUser : UserIdentity<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            validNotNullField(userFields, _fieldId);
            var id = (TKey)Convert.ChangeType(userFields[_fieldId], typeof(TKey));
            if (id.Equals(default))
            {
                throw new ValidationException($"Invalid field '{_fieldId}'");
            }

            validNotNullField(userFields, _fieldIdExt);
            Guid idExt;
            if (!Guid.TryParse(userFields[_fieldIdExt].ToString(), out idExt))
            {
                throw new ValidationException($"Invalid field '{_fieldIdExt}'");
            }

            validNotNullField(userFields, _fieldPassword);
            password = userFields[_fieldPassword]?.ToString();
            if (string.IsNullOrWhiteSpace(password))
                throw new ValidationException($"Invalid field '{_fieldPassword}'");

            var email = string.Empty;
            var phone = string.Empty;
            if (userFields.ContainsKey(_fieldEmail))
            {
                email = userFields[_fieldEmail]?.ToString();
            }
            if (userFields.ContainsKey(_fieldPhone))
            {
                phone = userFields[_fieldPhone]?.ToString();
            }
            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
                throw new ValidationException($"Fields '{_fieldEmail}' and '{_fieldPhone}' is not defined");

            var login = userFields[_fieldLogin]?.ToString();
            if (string.IsNullOrWhiteSpace(login))
            {
                login = string.IsNullOrWhiteSpace(email) ? phone : email;
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var vEmail = new EmailAddressAttribute();
                if (!vEmail.IsValid(email))
                    throw new ValidationException($"Invalid field '{_fieldEmail}'. {vEmail.ErrorMessage}");
            }
            if (!string.IsNullOrWhiteSpace(phone))
            {
                var vPhone = new PhoneAttribute();
                if (!vPhone.IsValid(phone))
                    throw new ValidationException($"Invalid field '{_fieldPhone}'. {vPhone.ErrorMessage}");
            }

            return new TUser
            {
                Id = id,
                Idext = idExt,
                UserName = login,
                Email = email,
                PhoneNumber = phone
            };
        }

        public static Claim[] GetClaims<TUserKey>(Dictionary<string, object> userFields) where TUserKey : IEquatable<TUserKey>
        {
            return userFields.Where(u => u.Value != null && u.Key != _fieldId && u.Key != _fieldIdExt
                && u.Key != _fieldLogin && u.Key != _fieldPassword
                && u.Key != _fieldEmail && u.Key != _fieldPhone).Select(u => new Claim(u.Key, u.Value.ToString())).ToArray();
        }

        public static IList<Claim> ExtractClaimsToRemove(Claim[] claimsOld, Claim[] claimsNew)
        {
            var claimsToRemove = new List<Claim>();
            foreach (var claimOld in claimsOld)
            {
                var claimNew = claimsNew.FirstOrDefault(c => c.Type == claimOld.Type);
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
                var claimOld = claimsOld.FirstOrDefault(c => c.Type == claimNew.Type);
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
                var claimNew = claimsNew.FirstOrDefault(c => c.Type == claimOld.Type);
                if (claimNew != null && !string.IsNullOrWhiteSpace(claimNew.Value))
                {
                    claimsToReplace.Add(new Tuple<Claim, Claim>(claimOld, claimNew));
                }
            }
            return claimsToReplace;
        }
    }
}
