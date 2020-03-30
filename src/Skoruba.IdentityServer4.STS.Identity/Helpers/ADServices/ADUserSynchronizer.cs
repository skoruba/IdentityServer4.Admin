using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers.ADServices
{
    public class ADUserSynchronizer<TUser, TKey> : IExternalUserSynchronizer
        where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly ILogger<ADUserSynchronizer<TUser, TKey>> _logger;
        private readonly ADUserInfoExtractor _adUserInfoExtractor;
        private readonly UserManager<TUser> _userManager;
        private readonly IOptions<WindowsAuthConfiguration> _adOptions;

        public ADUserSynchronizer(
            ILogger<ADUserSynchronizer<TUser, TKey>> logger,
            ADUserInfoExtractor adUserInfoExtractor,
            UserManager<TUser> userManager,
            IOptions<WindowsAuthConfiguration> adOptions
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _adUserInfoExtractor = adUserInfoExtractor ?? throw new ArgumentNullException(nameof(adUserInfoExtractor));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _adOptions = adOptions ?? throw new ArgumentNullException(nameof(adOptions));
        }

        public string LoginProvider { get { return AccountOptions.WindowsAuthenticationSchemeName; } }

        public async Task<UsersSynchronizationResult> SynchronizeAll(CancellationToken cancellationToken = default)
        {
            UsersSynchronizationResult result = new UsersSynchronizationResult();
            if (_adOptions.Value != null && _adOptions.Value.Domains != null)
            {
                foreach (var domainConfiguration in _adOptions.Value.Domains)
                {
                    var adUsersInfos = _adUserInfoExtractor.GetAllUsersFromAD(domainConfiguration);
                    foreach (var adUserInfo in adUsersInfos)
                    {
                        var oneSyncResult = await SynchronizeOne(adUserInfo, cancellationToken);
                        if (oneSyncResult.Created)
                            result.NewUsersCount++;
                        else if (oneSyncResult.Updated)
                            result.UpdatedUsersCount++;
                        else
                            result.SyncErrorsCount++;
                    }
                }
            }
            return result;
        }

        public async Task<UserSynchronizationResult<TUser>> SynchronizeOne(ADUserInfo adUserInfo, CancellationToken cancellationToken = default)
        {
            var result = new UserSynchronizationResult<TUser>();
            var user = await _userManager.FindByLoginAsync(AccountOptions.WindowsAuthenticationSchemeName, adUserInfo.UsernameFQDN);
            if (user != null)
            {
                user.Email = adUserInfo.Email;
                user.NormalizedEmail = adUserInfo.Email?.ToUpper();
                user.EmailConfirmed = !string.IsNullOrEmpty(adUserInfo.Email);
                user.PhoneNumber = adUserInfo.PhoneNumber;

                var currentClaims = await _userManager.GetClaimsAsync(user);
                await UpdateUserClaim(user, currentClaims, JwtClaimTypes.Email, adUserInfo.Email);
                await UpdateUserClaim(user, currentClaims, JwtClaimTypes.Picture, adUserInfo.Photo);
                await UpdateUserClaim(user, currentClaims, JwtClaimTypes.WebSite, adUserInfo.WebSite);
                await UpdateUserClaim(user, currentClaims, JwtClaimTypes.Address,
                    (!string.IsNullOrEmpty(adUserInfo.Country) || !string.IsNullOrEmpty(adUserInfo.StreetAddress)) ?
                    Newtonsoft.Json.JsonConvert.SerializeObject(new { country = adUserInfo.Country, street_address = adUserInfo.StreetAddress }) :
                    null);

                var currentRolesClaim = currentClaims.Where(c => c.Type == JwtClaimTypes.Role);
                // Remove the groups that the user doesn't belong to anymore.
                List<Claim> rolesToBeRemoved = new List<Claim>();
                foreach (var currentRoleClaim in currentRolesClaim)
                {
                    if (!adUserInfo.Groups.Contains(currentRoleClaim.Value))
                        rolesToBeRemoved.Add(currentRoleClaim);
                }
                var removeClaimRes = await _userManager.RemoveClaimsAsync(user, rolesToBeRemoved);
                if (!removeClaimRes.Succeeded)
                {
                    var errorDescrition = string.Join(Environment.NewLine, removeClaimRes.Errors.Select(s => $"{s.Description} (code #{s.Code})"));
                    _logger.LogError("Error when removing roles from user {userName}. Error description: " + errorDescrition, user.UserName);
                    result.Error = true;
                    return result;
                }
                //Add new groups that are not synced with the user
                List<Claim> rolesToBeAdded = new List<Claim>();
                foreach (var groupName in adUserInfo.Groups)
                {
                    var roleFound = currentRolesClaim.Where(p => p.Value == groupName).FirstOrDefault();
                    if (roleFound == null)
                        rolesToBeAdded.Add(new Claim(JwtClaimTypes.Role, groupName));
                }
                var addedClaimRes = await _userManager.AddClaimsAsync(user, rolesToBeAdded);
                if (!addedClaimRes.Succeeded)
                {
                    var errorDescrition = string.Join(Environment.NewLine, addedClaimRes.Errors.Select(s => $"{s.Description} (code #{s.Code})"));
                    _logger.LogError("Error when adding roles to user {userName}. Error description: " + errorDescrition, user.UserName);
                    result.Error = true;
                    return result;
                }

                var updateUserRes = await _userManager.UpdateAsync(user);
                if (!updateUserRes.Succeeded)
                {
                    var errorDescrition = string.Join(Environment.NewLine, updateUserRes.Errors.Select(s => $"{s.Description} (code #{s.Code})"));
                    _logger.LogError("Error when syncing user {userName} profile. Error description: " + errorDescrition, user.UserName);
                    result.Error = true;
                    return result;
                }
                result.Updated = true;
            }
            else
            {
                var claimsToAdd = new List<Claim>();

                if (!string.IsNullOrEmpty(adUserInfo.ObjectGuid))
                    claimsToAdd.Add(new Claim(JwtClaimTypes.Id, adUserInfo.ObjectGuid));
                var roles = adUserInfo.Groups.Select(x => new Claim(JwtClaimTypes.Role, x));
                claimsToAdd.AddRange(roles);

                if (!string.IsNullOrEmpty(adUserInfo.DisplayName))
                    claimsToAdd.Add(new Claim(JwtClaimTypes.Name, adUserInfo.DisplayName));
                if (!string.IsNullOrEmpty(adUserInfo.Email))
                    claimsToAdd.Add(new Claim(JwtClaimTypes.Email, adUserInfo.Email));
                if (!string.IsNullOrEmpty(adUserInfo.Photo))
                    claimsToAdd.Add(new Claim(JwtClaimTypes.Picture, adUserInfo.Photo));
                if (!string.IsNullOrEmpty(adUserInfo.WebSite))
                    claimsToAdd.Add(new Claim(JwtClaimTypes.WebSite, adUserInfo.WebSite));
                if (!string.IsNullOrEmpty(adUserInfo.Country) || !string.IsNullOrEmpty(adUserInfo.StreetAddress))
                    claimsToAdd.Add(new Claim(JwtClaimTypes.Address,
                        JsonSerializer.Serialize(new { country = adUserInfo.Country, street_address = adUserInfo.StreetAddress })));

                user = new TUser
                {
                    UserName = adUserInfo.Username,
                    Email = adUserInfo.Email,
                    NormalizedEmail = adUserInfo.Email?.ToUpper(),
                    EmailConfirmed = !string.IsNullOrEmpty(adUserInfo.Email),
                    PhoneNumber = adUserInfo.PhoneNumber
                };
                var identityResult = await _userManager.CreateAsync(user);
                if (!identityResult.Succeeded)
                {
                    var errorDescrition = string.Join(Environment.NewLine, identityResult.Errors.Select(s => $"{s.Description} (code #{s.Code})"));
                    _logger.LogError("Error when creating user {userName} profile. Error description: " + errorDescrition, user.UserName);
                    result.Error = true;
                    return result;
                }

                if (claimsToAdd.Any())
                {
                    identityResult = await _userManager.AddClaimsAsync(user, claimsToAdd);
                    if (!identityResult.Succeeded)
                    {
                        var errorDescrition = string.Join(Environment.NewLine, identityResult.Errors.Select(s => $"{s.Description} (code #{s.Code})"));
                        _logger.LogError("Error when creating user {userName} claims. Error description: " + errorDescrition, user.UserName);
                        result.Error = true;
                        return result;
                    }
                }

                var windowsLogin = new UserLoginInfo(AccountOptions.WindowsAuthenticationSchemeName, adUserInfo.UsernameFQDN, adUserInfo.DisplayName);
                identityResult = await _userManager.AddLoginAsync(user, windowsLogin);
                if (!identityResult.Succeeded)
                {
                    var errorDescrition = string.Join(Environment.NewLine, identityResult.Errors.Select(s => $"{s.Description} (code #{s.Code})"));
                    _logger.LogError("Error when creating user {userName} claims. Error description: " + errorDescrition, user.UserName);
                    result.Error = true;
                    return result;
                }
                result.Created = true;
            }
            result.User = user;
            return result;
        }

        private async Task<IdentityResult> UpdateUserClaim(TUser user, IList<Claim> currentClaims, string claimType, string claimNewValue)
        {
            IdentityResult res = IdentityResult.Success;
            var currentClaim = currentClaims.FirstOrDefault(c => c.Type == claimType);
            if (currentClaim != null)
            {
                if (string.IsNullOrEmpty(claimNewValue))
                    res = await _userManager.RemoveClaimAsync(user, currentClaim);
                else if (currentClaim.Value != claimNewValue)
                    res = await _userManager.ReplaceClaimAsync(user, currentClaim, new Claim(claimType, claimNewValue));
            }
            else if (!string.IsNullOrEmpty(claimNewValue))
                res = await _userManager.AddClaimAsync(user, new Claim(claimType, claimNewValue));

            if (!res.Succeeded)
            {
                var errorDescrition = string.Join(Environment.NewLine, res.Errors.Select(s => $"{s.Description} (code #{s.Code})"));
                _logger.LogError("Error when updating user {userName} claims. Error description: " + errorDescrition, user.UserName);
            }
            return res;
        }
    }
}
