using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Iserv.IdentityServer4.BusinessLogic.Helpers;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Iserv.IdentityServer4.BusinessLogic.Services
{
    public class ExportIdentityService<TUser, TKey> : IExportIdentityService
        where TUser : UserIdentity<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;

        public ExportIdentityService(UserManager<TUser> userManager)
        {
            _userManager = userManager;
        }

        private async Task updateUserAsync(object objUser)
        {
            if (objUser == null)
                throw new UserFriendlyErrorPageException("Data is incorrect");
            var userFields = JsonConvert.DeserializeObject<Dictionary<string, object>>(objUser.ToString());

            string password;
            var userBase = OpenIdClaimHelpers.GetUserBase<TUser, TKey>(userFields, out password);
            var claimsNew = OpenIdClaimHelpers.GetClaims<TKey>(userFields);

            var user = await _userManager.FindByIdAsync(userBase.Id.ToString());
            if (user == null)
            {
                user = new TUser
                {
                    Id = userBase.Id,
                    Idext = userBase.Idext,
                    UserName = userBase.UserName,
                    Email = userBase.Email,
                    PhoneNumber = userBase.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    throw new UserFriendlyErrorPageException(string.Join("\n", result.Errors.Select(er => er.Description)));
                result = await _userManager.AddClaimsAsync(user, claimsNew);
                if (!result.Succeeded)
                    throw new UserFriendlyErrorPageException(string.Join("\n", result.Errors.Select(er => er.Description)));
            }
            else
            {
                var claimsOld = (await _userManager.GetClaimsAsync(user)).ToArray();
                var claimsToRemove = OpenIdClaimHelpers.ExtractClaimsToRemove(claimsOld, claimsNew);
                var claimsToAdd = OpenIdClaimHelpers.ExtractClaimsToAdd(claimsOld, claimsNew);
                var claimsToReplace = OpenIdClaimHelpers.ExtractClaimsToReplace(claimsOld, claimsNew);

                var result = await _userManager.RemoveClaimsAsync(user, claimsToRemove);
                if (!result.Succeeded)
                    throw new UserFriendlyErrorPageException(string.Join("\n", result.Errors.Select(er => er.Description)));
                result = await _userManager.AddClaimsAsync(user, claimsToAdd);
                if (!result.Succeeded)
                    throw new UserFriendlyErrorPageException(string.Join("\n", result.Errors.Select(er => er.Description)));

                foreach (var pair in claimsToReplace)
                {
                    result = await _userManager.ReplaceClaimAsync(user, pair.Item1, pair.Item2);
                    if (!result.Succeeded)
                        throw new UserFriendlyErrorPageException(string.Join("\n", result.Errors.Select(er => er.Description)));
                }
            }
        }

        public async Task ImportUsersAsync(string txt)
        {
            if (string.IsNullOrWhiteSpace(txt))
            {
                throw new UserFriendlyErrorPageException("The file is not a configuration");
            }
            var users = JsonConvert.DeserializeObject<dynamic[]>(txt);
            if (users == null)
            {
                throw new UserFriendlyErrorPageException("The file is not a users");
            }
            foreach (object item in users)
            {
                await updateUserAsync(item);
            }
        }
    }
}