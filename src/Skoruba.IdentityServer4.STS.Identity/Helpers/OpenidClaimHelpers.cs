using IdentityModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using Skoruba.IdentityServer4.STS.Identity.Configuration.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public static class OpenIdClaimHelpers
    {
        public static Claim ExtractAddressClaim(OpenIdProfile profile)
        {
            var addressJson = new JObject();
            if (!string.IsNullOrWhiteSpace(profile.StreetAddress))
            {
                addressJson[AddressClaimConstants.StreetAddress] = profile.StreetAddress;
            }

            if (!string.IsNullOrWhiteSpace(profile.Locality))
            {
                addressJson[AddressClaimConstants.Locality] = profile.Locality;
            }

            if (!string.IsNullOrWhiteSpace(profile.Region))
            {
                addressJson[AddressClaimConstants.Region] = profile.Region;
            }

            if (!string.IsNullOrWhiteSpace(profile.PostalCode))
            {
                addressJson[AddressClaimConstants.PostalCode] = profile.PostalCode;
            }

            if (!string.IsNullOrWhiteSpace(profile.Country))
            {
                addressJson[AddressClaimConstants.Country] = profile.Country;
            }


            return new Claim(JwtClaimTypes.Address, addressJson.Count != 0 ? addressJson.ToString() : string.Empty);
        }

        /// <summary>
        /// Map claims to OpenId Profile
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public static OpenIdProfile ExtractProfileInfo(IList<Claim> claims)
        {
            var profile = new OpenIdProfile
            {
                FirstName = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.FirstName)?.Value,
                Name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value,
                MiddleName = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.MiddleName)?.Value,
                //Gender = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Gender)?.Value == "female" ? EGenders.Female : EGenders.Male,
                UserType = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.UserType)?.Value,
                SignNotifyActive = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.SignNotifyActive)?.Value == "true",
                SignPushNotifyActive = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.SignPushNotifyActive)?.Value == "true",
                RegionAddrRef = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.RegionAddrRef)?.Value,
                UserFromEsia = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.UserFromEsia)?.Value == "true",
                SignAutoLocationActive = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.SignAutoLocationActive)?.Value == "true",
                AddressFias = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.AddressFias)?.Value,
                LoginNameIp = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.LoginNameIp)?.Value,
                LoginNameUl = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.LoginNameUl)?.Value,
                NameOrg = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.NameOrg)?.Value,
                OgrnIp = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.OgrnIp)?.Value,
                OgrnUl = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.OgrnUl)?.Value,
                Opf = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.Opf)?.Value,
                ShowSvetAttributes = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.ShowSvetAttributes)?.Value == "true",
                ShowExtendedAttributes = claims.FirstOrDefault(x => x.Type == JwtClaimTypesExtra.ShowExtendedAttributes)?.Value == "true",
                Website = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.WebSite)?.Value,
                Profile = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Profile)?.Value
            };

            var address = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Address)?.Value;

            if (address == null)
                return profile;

            try
            {
                var addressJson = JObject.Parse(address);
                if (addressJson.ContainsKey(AddressClaimConstants.StreetAddress))
                {
                    profile.StreetAddress = addressJson[AddressClaimConstants.StreetAddress].ToString();
                }

                if (addressJson.ContainsKey(AddressClaimConstants.Locality))
                {
                    profile.Locality = addressJson[AddressClaimConstants.Locality].ToString();
                }

                if (addressJson.ContainsKey(AddressClaimConstants.Region))
                {
                    profile.Region = addressJson[AddressClaimConstants.Region].ToString();
                }

                if (addressJson.ContainsKey(AddressClaimConstants.PostalCode))
                {
                    profile.PostalCode = addressJson[AddressClaimConstants.PostalCode].ToString();
                }

                if (addressJson.ContainsKey(AddressClaimConstants.Country))
                {
                    profile.Country = addressJson[AddressClaimConstants.Country].ToString();
                }
            }
            catch (JsonReaderException)
            {

            }

            return profile;
        }

        /// <summary>
        /// Get claims to remove
        /// </summary>
        /// <param name="oldProfile"></param>
        /// <param name="newProfile"></param>
        /// <returns></returns>
        public static IList<Claim> ExtractClaimsToRemove(OpenIdProfile oldProfile, OpenIdProfile newProfile)
        {
            var claimsToRemove = new List<Claim>();

            if (string.IsNullOrWhiteSpace(newProfile.FirstName) && !string.IsNullOrWhiteSpace(oldProfile.FirstName))
            {
                claimsToRemove.Add(new Claim(JwtClaimTypesExtra.FirstName, oldProfile.FirstName));
            }
            if (string.IsNullOrWhiteSpace(newProfile.Name) && !string.IsNullOrWhiteSpace(oldProfile.Name))
            {
                claimsToRemove.Add(new Claim(JwtClaimTypes.Name, oldProfile.Name));
            }
            if (string.IsNullOrWhiteSpace(newProfile.MiddleName) && !string.IsNullOrWhiteSpace(oldProfile.MiddleName))
            {
                claimsToRemove.Add(new Claim(JwtClaimTypes.MiddleName, oldProfile.MiddleName));
            }
            //claimsToRemove.Add(new Claim(JwtClaimTypes.Gender, Enum.GetName(typeof(EGenders), oldProfile.Gender).ToLower()));
            if (string.IsNullOrWhiteSpace(newProfile.UserType) && !string.IsNullOrWhiteSpace(oldProfile.UserType))
            {
                claimsToRemove.Add(new Claim(JwtClaimTypesExtra.UserType, oldProfile.UserType));
            }
            claimsToRemove.Add(new Claim(JwtClaimTypesExtra.SignNotifyActive, oldProfile.SignNotifyActive.ToString().ToLower()));
            claimsToRemove.Add(new Claim(JwtClaimTypesExtra.SignPushNotifyActive, oldProfile.SignPushNotifyActive.ToString().ToLower()));
            if (string.IsNullOrWhiteSpace(newProfile.RegionAddrRef) && !string.IsNullOrWhiteSpace(oldProfile.RegionAddrRef))
            {
                claimsToRemove.Add(new Claim(JwtClaimTypesExtra.RegionAddrRef, oldProfile.RegionAddrRef));
            }
            claimsToRemove.Add(new Claim(JwtClaimTypesExtra.UserFromEsia, oldProfile.UserFromEsia.ToString().ToLower()));
            claimsToRemove.Add(new Claim(JwtClaimTypesExtra.SignAutoLocationActive, oldProfile.SignAutoLocationActive.ToString().ToLower()));
            if (string.IsNullOrWhiteSpace(newProfile.AddressFias) && !string.IsNullOrWhiteSpace(oldProfile.AddressFias))
            {
                claimsToRemove.Add(new Claim(JwtClaimTypesExtra.AddressFias, oldProfile.AddressFias));
            }
            if (string.IsNullOrWhiteSpace(newProfile.LoginNameIp) && !string.IsNullOrWhiteSpace(oldProfile.LoginNameIp))
            {
                claimsToRemove.Add(new Claim(JwtClaimTypesExtra.LoginNameIp, oldProfile.LoginNameIp));
            }
            if (string.IsNullOrWhiteSpace(newProfile.LoginNameUl) && !string.IsNullOrWhiteSpace(oldProfile.LoginNameUl))
            {
                claimsToRemove.Add(new Claim(JwtClaimTypesExtra.LoginNameUl, oldProfile.LoginNameUl));
            }
            if (string.IsNullOrWhiteSpace(newProfile.NameOrg) && !string.IsNullOrWhiteSpace(oldProfile.NameOrg))
            {
                claimsToRemove.Add(new Claim(JwtClaimTypesExtra.NameOrg, oldProfile.NameOrg));
            }
            if (string.IsNullOrWhiteSpace(newProfile.OgrnIp) && !string.IsNullOrWhiteSpace(oldProfile.OgrnIp))
            {
                claimsToRemove.Add(new Claim(JwtClaimTypesExtra.OgrnIp, oldProfile.OgrnIp));
            }
            if (string.IsNullOrWhiteSpace(newProfile.OgrnUl) && !string.IsNullOrWhiteSpace(oldProfile.OgrnUl))
            {
                claimsToRemove.Add(new Claim(JwtClaimTypesExtra.OgrnUl, oldProfile.OgrnUl));
            }
            if (string.IsNullOrWhiteSpace(newProfile.Opf) && !string.IsNullOrWhiteSpace(oldProfile.Opf))
            {
                claimsToRemove.Add(new Claim(JwtClaimTypesExtra.Opf, oldProfile.Opf));
            }
            claimsToRemove.Add(new Claim(JwtClaimTypesExtra.ShowSvetAttributes, oldProfile.ShowSvetAttributes.ToString().ToLower()));
            claimsToRemove.Add(new Claim(JwtClaimTypesExtra.ShowExtendedAttributes, oldProfile.ShowExtendedAttributes.ToString().ToLower()));

            if (string.IsNullOrWhiteSpace(newProfile.Website) && !string.IsNullOrWhiteSpace(oldProfile.Website))
            {
                claimsToRemove.Add(new Claim(JwtClaimTypes.WebSite, oldProfile.Website));
            }

            if (string.IsNullOrWhiteSpace(newProfile.Profile) && !string.IsNullOrWhiteSpace(oldProfile.Profile))
            {
                claimsToRemove.Add(new Claim(JwtClaimTypes.Profile, oldProfile.Profile));
            }

            var oldAddressClaim = ExtractAddressClaim(oldProfile);
            var newAddressClaim = ExtractAddressClaim(newProfile);

            if (string.IsNullOrWhiteSpace(newAddressClaim.Value) && !string.IsNullOrWhiteSpace(oldAddressClaim.Value))
            {
                claimsToRemove.Add(oldAddressClaim);
            }

            return claimsToRemove;
        }

        /// <summary>
        /// Get claims to add
        /// </summary>
        /// <param name="oldProfile"></param>
        /// <param name="newProfile"></param>
        /// <returns></returns>
        public static IList<Claim> ExtractClaimsToAdd(OpenIdProfile oldProfile, OpenIdProfile newProfile)
        {
            var claimsToAdd = new List<Claim>();

            if (!string.IsNullOrWhiteSpace(newProfile.FirstName) && string.IsNullOrWhiteSpace(oldProfile.FirstName))
            {
                claimsToAdd.Add(new Claim(JwtClaimTypesExtra.FirstName, newProfile.FirstName));
            }
            if (!string.IsNullOrWhiteSpace(newProfile.Name) && string.IsNullOrWhiteSpace(oldProfile.Name))
            {
                claimsToAdd.Add(new Claim(JwtClaimTypes.Name, newProfile.Name));
            }
            if (!string.IsNullOrWhiteSpace(newProfile.MiddleName) && string.IsNullOrWhiteSpace(oldProfile.MiddleName))
            {
                claimsToAdd.Add(new Claim(JwtClaimTypes.MiddleName, newProfile.MiddleName));
            }
            //claimsToAdd.Add(new Claim(JwtClaimTypes.Gender, newProfile.Gender.ToString().ToLower()));
            if (!string.IsNullOrWhiteSpace(newProfile.UserType) && string.IsNullOrWhiteSpace(oldProfile.UserType))
            {
                claimsToAdd.Add(new Claim(JwtClaimTypesExtra.UserType, newProfile.UserType));
            }
            claimsToAdd.Add(new Claim(JwtClaimTypesExtra.SignNotifyActive, newProfile.SignNotifyActive.ToString().ToLower()));
            claimsToAdd.Add(new Claim(JwtClaimTypesExtra.SignPushNotifyActive, newProfile.SignPushNotifyActive.ToString().ToLower()));
            if (!string.IsNullOrWhiteSpace(newProfile.RegionAddrRef) && string.IsNullOrWhiteSpace(oldProfile.RegionAddrRef))
            {
                claimsToAdd.Add(new Claim(JwtClaimTypesExtra.RegionAddrRef, newProfile.RegionAddrRef));
            }
            claimsToAdd.Add(new Claim(JwtClaimTypesExtra.UserFromEsia, newProfile.UserFromEsia.ToString().ToLower()));
            claimsToAdd.Add(new Claim(JwtClaimTypesExtra.SignAutoLocationActive, newProfile.SignAutoLocationActive.ToString().ToLower()));
            if (!string.IsNullOrWhiteSpace(newProfile.AddressFias) && string.IsNullOrWhiteSpace(oldProfile.AddressFias))
            {
                claimsToAdd.Add(new Claim(JwtClaimTypesExtra.AddressFias, newProfile.AddressFias));
            }
            if (!string.IsNullOrWhiteSpace(newProfile.LoginNameIp) && string.IsNullOrWhiteSpace(oldProfile.LoginNameIp))
            {
                claimsToAdd.Add(new Claim(JwtClaimTypesExtra.LoginNameIp, newProfile.LoginNameIp));
            }
            if (!string.IsNullOrWhiteSpace(newProfile.LoginNameUl) && string.IsNullOrWhiteSpace(oldProfile.LoginNameUl))
            {
                claimsToAdd.Add(new Claim(JwtClaimTypesExtra.LoginNameUl, newProfile.LoginNameUl));
            }
            if (!string.IsNullOrWhiteSpace(newProfile.NameOrg) && string.IsNullOrWhiteSpace(oldProfile.NameOrg))
            {
                claimsToAdd.Add(new Claim(JwtClaimTypesExtra.NameOrg, newProfile.NameOrg));
            }
            if (!string.IsNullOrWhiteSpace(newProfile.OgrnIp) && string.IsNullOrWhiteSpace(oldProfile.OgrnIp))
            {
                claimsToAdd.Add(new Claim(JwtClaimTypesExtra.OgrnIp, newProfile.OgrnIp));
            }
            if (!string.IsNullOrWhiteSpace(newProfile.OgrnUl) && string.IsNullOrWhiteSpace(oldProfile.OgrnUl))
            {
                claimsToAdd.Add(new Claim(JwtClaimTypesExtra.OgrnUl, newProfile.OgrnUl));
            }
            if (!string.IsNullOrWhiteSpace(newProfile.Opf) && string.IsNullOrWhiteSpace(oldProfile.Opf))
            {
                claimsToAdd.Add(new Claim(JwtClaimTypesExtra.Opf, newProfile.Opf));
            }
            claimsToAdd.Add(new Claim(JwtClaimTypesExtra.ShowSvetAttributes, newProfile.ShowSvetAttributes.ToString().ToLower()));
            claimsToAdd.Add(new Claim(JwtClaimTypesExtra.ShowSvetAttributes, newProfile.ShowSvetAttributes.ToString().ToLower()));

            if (!string.IsNullOrWhiteSpace(newProfile.Website) && string.IsNullOrWhiteSpace(oldProfile.Website))
            {
                claimsToAdd.Add(new Claim(JwtClaimTypes.WebSite, newProfile.Website));
            }

            if (!string.IsNullOrWhiteSpace(newProfile.Profile) && string.IsNullOrWhiteSpace(oldProfile.Profile))
            {
                claimsToAdd.Add(new Claim(JwtClaimTypes.Profile, newProfile.Profile));
            }

            var oldAddressClaim = ExtractAddressClaim(oldProfile);
            var newAddressClaim = ExtractAddressClaim(newProfile);

            if (!string.IsNullOrWhiteSpace(newAddressClaim.Value) && string.IsNullOrWhiteSpace(oldAddressClaim.Value))
            {
                claimsToAdd.Add(newAddressClaim);
            }

            return claimsToAdd;
        }

        /// <summary>
        /// Get claims to replace
        /// </summary>
        /// <param name="oldClaims"></param>
        /// <param name="newProfile"></param>
        /// <returns></returns>
        public static IList<Tuple<Claim, Claim>> ExtractClaimsToReplace(IList<Claim> oldClaims, OpenIdProfile newProfile)
        {
            var oldProfile = ExtractProfileInfo(oldClaims);
            var claimsToReplace = new List<Tuple<Claim, Claim>>();

            if (!string.IsNullOrWhiteSpace(newProfile.FirstName) && !string.IsNullOrWhiteSpace(oldProfile.FirstName))
            {
                if (newProfile.FirstName != oldProfile.FirstName)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.FirstName);
                    var newClaim = new Claim(JwtClaimTypesExtra.FirstName, newProfile.FirstName);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
                }
            }
            if (!string.IsNullOrWhiteSpace(newProfile.Name) && !string.IsNullOrWhiteSpace(oldProfile.Name))
            {
                if (newProfile.Name != oldProfile.Name)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypes.Name);
                    var newClaim = new Claim(JwtClaimTypes.Name, newProfile.Name);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
                }
            }
            if (!string.IsNullOrWhiteSpace(newProfile.MiddleName) && !string.IsNullOrWhiteSpace(oldProfile.MiddleName))
            {
                if (newProfile.MiddleName != oldProfile.MiddleName)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypes.MiddleName);
                    var newClaim = new Claim(JwtClaimTypes.MiddleName, newProfile.MiddleName);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
                }
            }
            //if (newProfile.Gender != oldProfile.Gender)
            //{
            //    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypes.Gender);
            //    var newClaim = new Claim(JwtClaimTypes.Gender, newProfile.Gender.ToString().ToLower());
            //    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
            //}
            if (!string.IsNullOrWhiteSpace(newProfile.UserType) && !string.IsNullOrWhiteSpace(oldProfile.UserType))
            {
                if (newProfile.UserType != oldProfile.UserType)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.UserType);
                    var newClaim = new Claim(JwtClaimTypesExtra.UserType, newProfile.UserType);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
                }
            }
            if (newProfile.SignNotifyActive != oldProfile.SignNotifyActive)
            {
                var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.SignNotifyActive);
                var newClaim = new Claim(JwtClaimTypesExtra.SignNotifyActive, newProfile.SignNotifyActive.ToString().ToLower());
                claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
            }
            if (newProfile.SignPushNotifyActive != oldProfile.SignPushNotifyActive)
            {
                var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.SignPushNotifyActive);
                var newClaim = new Claim(JwtClaimTypesExtra.SignPushNotifyActive, newProfile.SignPushNotifyActive.ToString().ToLower());
                claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
            }
            if (!string.IsNullOrWhiteSpace(newProfile.RegionAddrRef) && !string.IsNullOrWhiteSpace(oldProfile.RegionAddrRef))
            {
                if (newProfile.RegionAddrRef != oldProfile.RegionAddrRef)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.RegionAddrRef);
                    var newClaim = new Claim(JwtClaimTypesExtra.RegionAddrRef, newProfile.RegionAddrRef);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
                }
            }
            if (newProfile.UserFromEsia != oldProfile.UserFromEsia)
            {
                var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.UserFromEsia);
                var newClaim = new Claim(JwtClaimTypesExtra.UserFromEsia, newProfile.UserFromEsia.ToString().ToLower());
                claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
            }
            if (newProfile.SignAutoLocationActive != oldProfile.SignAutoLocationActive)
            {
                var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.SignAutoLocationActive);
                var newClaim = new Claim(JwtClaimTypesExtra.SignAutoLocationActive, newProfile.SignAutoLocationActive.ToString().ToLower());
                claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
            }
            if (!string.IsNullOrWhiteSpace(newProfile.AddressFias) && !string.IsNullOrWhiteSpace(oldProfile.AddressFias))
            {
                if (newProfile.AddressFias != oldProfile.AddressFias)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.AddressFias);
                    var newClaim = new Claim(JwtClaimTypesExtra.AddressFias, newProfile.AddressFias);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
                }
            }
            if (!string.IsNullOrWhiteSpace(newProfile.LoginNameIp) && !string.IsNullOrWhiteSpace(oldProfile.LoginNameIp))
            {
                if (newProfile.LoginNameIp != oldProfile.LoginNameIp)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.LoginNameIp);
                    var newClaim = new Claim(JwtClaimTypesExtra.LoginNameIp, newProfile.LoginNameIp);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
                }
            }
            if (!string.IsNullOrWhiteSpace(newProfile.LoginNameUl) && !string.IsNullOrWhiteSpace(oldProfile.LoginNameUl))
            {
                if (newProfile.LoginNameUl != oldProfile.LoginNameUl)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.LoginNameUl);
                    var newClaim = new Claim(JwtClaimTypesExtra.LoginNameUl, newProfile.LoginNameUl);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
                }
            }
            if (!string.IsNullOrWhiteSpace(newProfile.NameOrg) && !string.IsNullOrWhiteSpace(oldProfile.NameOrg))
            {
                if (newProfile.NameOrg != oldProfile.NameOrg)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.NameOrg);
                    var newClaim = new Claim(JwtClaimTypesExtra.NameOrg, newProfile.NameOrg);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
                }
            }
            if (!string.IsNullOrWhiteSpace(newProfile.OgrnIp) && !string.IsNullOrWhiteSpace(oldProfile.OgrnIp))
            {
                if (newProfile.OgrnIp != oldProfile.OgrnIp)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.OgrnIp);
                    var newClaim = new Claim(JwtClaimTypesExtra.OgrnIp, newProfile.OgrnIp);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
                }
            }
            if (!string.IsNullOrWhiteSpace(newProfile.OgrnUl) && !string.IsNullOrWhiteSpace(oldProfile.OgrnUl))
            {
                if (newProfile.OgrnUl != oldProfile.OgrnUl)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.OgrnUl);
                    var newClaim = new Claim(JwtClaimTypesExtra.OgrnUl, newProfile.OgrnUl);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
                }
            }
            if (!string.IsNullOrWhiteSpace(newProfile.Opf) && !string.IsNullOrWhiteSpace(oldProfile.Opf))
            {
                if (newProfile.Opf != oldProfile.Opf)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.Opf);
                    var newClaim = new Claim(JwtClaimTypesExtra.Opf, newProfile.Opf);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
                }
            }
            if (newProfile.ShowSvetAttributes != oldProfile.ShowSvetAttributes)
            {
                var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.ShowSvetAttributes);
                var newClaim = new Claim(JwtClaimTypesExtra.ShowSvetAttributes, newProfile.ShowSvetAttributes.ToString().ToLower());
                claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
            }
            if (newProfile.ShowExtendedAttributes != oldProfile.ShowExtendedAttributes)
            {
                var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypesExtra.ShowExtendedAttributes);
                var newClaim = new Claim(JwtClaimTypesExtra.ShowExtendedAttributes, newProfile.ShowExtendedAttributes.ToString().ToLower());
                claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
            }


            if (!string.IsNullOrWhiteSpace(newProfile.Website) && !string.IsNullOrWhiteSpace(oldProfile.Website))
            {
                if (newProfile.Website != oldProfile.Website)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypes.WebSite);
                    var newClaim = new Claim(JwtClaimTypes.WebSite, newProfile.Website);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
                }
            }

            if (!string.IsNullOrWhiteSpace(newProfile.Profile) && !string.IsNullOrWhiteSpace(oldProfile.Profile))
            {
                if (newProfile.Profile != oldProfile.Profile)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypes.Profile);
                    var newClaim = new Claim(JwtClaimTypes.Profile, newProfile.Profile);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
                }
            }

            var oldAddressClaim = ExtractAddressClaim(oldProfile);
            var newAddressClaim = ExtractAddressClaim(newProfile);

            if (!string.IsNullOrWhiteSpace(newAddressClaim.Value) && !string.IsNullOrWhiteSpace(oldAddressClaim.Value))
            {
                if (newAddressClaim.Value != oldAddressClaim.Value)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypes.Address);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newAddressClaim));
                }
            }

            return claimsToReplace;
        }
    }
}
