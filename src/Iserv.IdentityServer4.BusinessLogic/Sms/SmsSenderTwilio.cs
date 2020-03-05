using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Iserv.IdentityServer4.BusinessLogic.Sms
{
    public class SmsSenderTwilio : ISmsService
    {
        private readonly SmsSetting _smsSetting;
        private readonly ILogger<SmsSenderTwilio> _logger;

        public SmsSenderTwilio(SmsSetting smsSetting, ILogger<SmsSenderTwilio> logger)
        {
            _smsSetting = smsSetting;
            _logger = logger;
        }

        public async Task<SmsResult> SendSmsAsync(string numberTo, string message)
        {
            return await SendSmsAsync(_smsSetting.DefaultPhoneFrom, numberTo, message);
        }

        public async Task<SmsResult> SendSmsAsync(string numberFrom, string numberTo, string message)
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
            if (result.ErrorCode == null) return new SmsResult() {Message = "Сообщение отправилось"};
            var msg = "Не удалось отправить сообщение на номер " + numberTo;
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                msg += ". " + result.ErrorMessage;
            }

            return new SmsResult {IsError = true, Message = msg};
        }

        public void Dispose()
        {
        }
    }
}