using IdentityServer4.AspNetIdentity;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Iserv.IdentityServer4.BusinessLogic.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using System;
using System.Threading.Tasks;

namespace Iserv.IdentityServer4.BusinessLogic.Validators
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ResourceOwnerValidatorPassword<TUser, TKey> : ResourceOwnerPasswordValidator<TUser>
        where TUser : UserIdentity<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IAccountService<TUser, TKey> _accountService;
        private readonly IPortalService _portalService;
        private readonly ILogger<ResourceOwnerPasswordValidator<TUser>> _logger;

        public ResourceOwnerValidatorPassword(UserManager<TUser> userManager, SignInManager<TUser> signInManager, IAccountService<TUser, TKey> accountService,
            IPortalService portalService, IEventService events, ILogger<ResourceOwnerPasswordValidator<TUser>> logger)
            : base(userManager, signInManager, events, logger)
        {
            _userManager = userManager;
            _accountService = accountService;
            _portalService = portalService;
            _logger = logger;
        }

        public override async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            await base.ValidateAsync(context);
            if (!context.Result.IsError)
                return;
            try
            {
                var resultPortalId = await _portalService.GetUserIdByAuthAsync(context.UserName, context.Password);
                if (resultPortalId.IsError)
                    return;
                var user = await _userManager.FindByNameAsync(context.UserName);
                if (user != null)
                {
                    var tokenPassword = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, tokenPassword, context.Password);
                    await base.ValidateAsync(context);
                }
                else
                {
                    if ((await _accountService.CreateUserFromPortalAsync(resultPortalId.Value, context.Password)).Succeeded)
                    {
                        await base.ValidateAsync(context);
                    }
                }
            }
            catch (Exception err)
            {
                context.Result.IsError = true;
                context.Result.ErrorDescription = err.Message;
                _logger.LogError(err.Message, err);
            }
        }
    }
}