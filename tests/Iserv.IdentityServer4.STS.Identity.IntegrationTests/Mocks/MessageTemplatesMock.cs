using Iserv.IdentityServer4.BusinessLogic.Settings;

namespace Iserv.IdentityServer4.STS.Identity.IntegrationTests.Mocks
{
    public static class MessageTemplatesMock
    {
        public static MessageTemplates GetMessageTemplates()
        {
            return new MessageTemplates()
            {
                CheckEmailTitle = "Подтверждение электронного адреса",
                CheckEmail = "Код для подтверждения электронного адреса: {0}",
                CheckPhoneNumberSms = "Код для подтверждения номера телефона: {0}",
                RepairPasswordSms = "Код для восстановления пароля: {0}"
            };
        }
    }
}