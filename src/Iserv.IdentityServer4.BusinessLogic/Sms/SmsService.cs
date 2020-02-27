using System;
using System.Threading.Tasks;

namespace Iserv.IdentityServer4.BusinessLogic.Sms
{
    public class SmsService : ISmsService
    {
        private readonly ISmsSender _smsSender;
        
        public SmsService(SmsSetting smsSetting)
        {
            var smsSetting1 = smsSetting;
            switch (smsSetting.Provider)
            {
                case SmsProvider.Devino:
                    _smsSender = new SmsSenderDevino(smsSetting1);
                    break;
                case SmsProvider.Twilio:
                    _smsSender = new SmsSenderTwilio(smsSetting1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public async Task<string> SendSmsAsync(string numberTo, string message)
        {
            return await _smsSender.SendSmsAsync(numberTo, message);
        }

        public async Task<string> SendSmsAsync(string numberFrom, string numberTo, string message)
        {
            return await _smsSender.SendSmsAsync(numberFrom, numberTo, message);
        }

        public void Dispose()
        {
            _smsSender.Dispose();
        }
    }
}