using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Sms
{
    public class SMSSenderTwilio : ISMSSender
    {
        private readonly SMSSetting _smsSetting;

        public SMSSenderTwilio(SMSSetting smsSetting)
        {
            _smsSetting = smsSetting;
        }

        public async Task<string> SendSMSAsync(string numberTo, string message)
        {
            return await SendSMSAsync(_smsSetting.DefaultPhoneFrom, numberTo, message);
        }

        public async Task<string> SendSMSAsync(string numberFrom, string numberTo, string message)
        {
            if (string.IsNullOrEmpty(numberFrom) || string.IsNullOrEmpty(numberTo) || string.IsNullOrEmpty(message))
            {
                return null;
            }
            TwilioClient.Init(_smsSetting.AccountSid, _smsSetting.AuthToken);
            var result = await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(numberFrom),
                to: new Twilio.Types.PhoneNumber(numberTo)
            );
            if (result.ErrorCode != null)
            {
                var msg = "Failed to send SMS to " + numberTo;
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    msg += ". " + result.ErrorMessage;
                }
                return msg;
            }
            return null;
        }

        public void Dispose()
        {

        }
    }
}
