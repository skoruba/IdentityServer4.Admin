using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Iserv.IdentityServer4.BusinessLogic.Sms
{
    public class SmsSenderTwilio : ISmsSender
    {
        private readonly SmsSetting _smsSetting;

        public SmsSenderTwilio(SmsSetting smsSetting)
        {
            _smsSetting = smsSetting;
        }

        public async Task<string> SendSmsAsync(string numberTo, string message)
        {
            return await SendSmsAsync(_smsSetting.DefaultPhoneFrom, numberTo, message);
        }

        public async Task<string> SendSmsAsync(string numberFrom, string numberTo, string message)
        {
            if (string.IsNullOrEmpty(numberFrom) || string.IsNullOrEmpty(numberTo) || string.IsNullOrEmpty(message))
            {
                return null;
            }
            TwilioClient.Init(_smsSetting.AccountSid, _smsSetting.AuthToken);
            var result = await MessageResource.CreateAsync(
                body: message,
                @from: new Twilio.Types.PhoneNumber(numberFrom),
                to: new Twilio.Types.PhoneNumber(numberTo)
            );
            if (result.ErrorCode == null) return null;
            var msg = "Failed to send SMS to " + numberTo;
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                msg += ". " + result.ErrorMessage;
            }
            return msg;
        }

        public void Dispose()
        {

        }
    }
}
