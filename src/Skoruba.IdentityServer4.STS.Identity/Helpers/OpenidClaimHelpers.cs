using IdentityModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                FullName = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value,
                Website = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.WebSite)?.Value,
                Profile = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Profile)?.Value
            };

            var address = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Address)?.Value;

            if (address == null) return profile;

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

            if (string.IsNullOrWhiteSpace(newProfile.FullName) && !string.IsNullOrWhiteSpace(oldProfile.FullName))
            {
                claimsToRemove.Add(new Claim(JwtClaimTypes.Name, oldProfile.FullName));
            }

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

            if (!string.IsNullOrWhiteSpace(newProfile.FullName) && string.IsNullOrWhiteSpace(oldProfile.FullName))
            {
                claimsToAdd.Add(new Claim(JwtClaimTypes.Name, newProfile.FullName));
            }

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
        public static IList<Tuple<Claim,Claim>> ExtractClaimsToReplace(IList<Claim> oldClaims, OpenIdProfile newProfile)
        {
            var oldProfile = ExtractProfileInfo(oldClaims);
            var claimsToReplace = new List<Tuple<Claim, Claim>>();

            if (!string.IsNullOrWhiteSpace(newProfile.FullName) && !string.IsNullOrWhiteSpace(oldProfile.FullName))
            {
                if (newProfile.FullName != oldProfile.FullName)
                {
                    var oldClaim = oldClaims.First(x => x.Type == JwtClaimTypes.Name);
                    var newClaim = new Claim(JwtClaimTypes.Name, newProfile.FullName);
                    claimsToReplace.Add(new Tuple<Claim, Claim>(oldClaim, newClaim));
                }
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
