using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.STS.Identity.MobileAuth.Services;
using Skoruba.IdentityServer4.STS.Identity.MobileAuth.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Skoruba.IdentityServer4.STS.Identity.Configuration.Constants.MobileAuthConstants;

namespace Skoruba.IdentityServer4.STS.Identity.Controllers
{
    [Route("api/[controller]")]
    public class MobileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISmsService _smsService;
        private readonly DataProtectorTokenProvider<UserIdentity> _dataProtectorTokenProvider;
        private readonly PhoneNumberTokenProvider<UserIdentity> _phoneNumberTokenProvider;
        private readonly UserManager<UserIdentity> _userManager;

        public MobileController(
            IConfiguration configuration,
            ISmsService smsService,
            DataProtectorTokenProvider<UserIdentity> dataProtectorTokenProvider,
            PhoneNumberTokenProvider<UserIdentity> phoneNumberTokenProvider,
            UserManager<UserIdentity> userManager)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            _dataProtectorTokenProvider = dataProtectorTokenProvider ?? throw new ArgumentNullException(nameof(dataProtectorTokenProvider));
            _phoneNumberTokenProvider = phoneNumberTokenProvider ?? throw new ArgumentNullException(nameof(phoneNumberTokenProvider));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }




        #region Mobile Auth (Generate OTP)
        [HttpPost]
        [Route("generate-otp")]
        public async Task<IActionResult> GenerateOTP([FromBody] PhoneLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await GetUser(model);
            if (user == null)
                return BadRequest("No User found in database on this phone no.");


            var response = await SendSmsRequet(model, user);

            if (!response.Result)
                return BadRequest("Sending sms failed");

            var protectToken = await _userManager.GenerateUserTokenAsync(user, "Default", TokenPurpose.MobilePasswordAuth);
            var body = GetBody(response.VerifyToken, protectToken);

            return Accepted(body);
        }

        [HttpPut]
        [Route("generate-otp")]
        public async Task<IActionResult> GenerateOTP([FromForm] string protectToken, [FromForm] PhoneLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await GetUser(model);
            if (!await _userManager.VerifyUserTokenAsync(user, "Default", TokenPurpose.MobilePasswordAuth, protectToken))
                return BadRequest("Invalid protect token");

            var response = await SendSmsRequet(model, user);

            if (!response.Result)
                return BadRequest("Sending sms failed");

            var newProtectToken = await _userManager.GenerateUserTokenAsync(user, "Default", TokenPurpose.MobilePasswordAuth);
            var body = GetBody(response.VerifyToken, newProtectToken);
            return Accepted(body);
        }

        async Task<UserIdentity> GetUser(PhoneLoginViewModel loginViewModel)
        {
            var phoneNumber = loginViewModel.PhoneNumber; 
            return await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        }

        async Task<(string VerifyToken, bool Result)> SendSmsRequet(PhoneLoginViewModel model, UserIdentity user)
        {
            var verifyToken = await _phoneNumberTokenProvider.GenerateAsync(TokenPurpose.MobilePasswordAuth, _userManager, user);
            var sms = $"<#>{Environment.NewLine}Your OTP Code is:{verifyToken}{Environment.NewLine}[{model.AppHash}]";

            var result = await _smsService.SendAsync(model.PhoneNumber, sms);
            return (verifyToken, result);
        }

        Dictionary<string, string> GetBody(string verifyToken, string protectToken)
        {
            var body = new Dictionary<string, string> { { TokenRequest.ProtectToken, protectToken } };

            if (_configuration["ReturnVerifyTokenForTesting"] == bool.TrueString)
                body.Add(TokenRequest.VerificationToken, verifyToken);

            return body;
        }
        #endregion
    }
}