using System;
using System.Threading.Tasks;

namespace Iserv.IdentityServer4.BusinessLogic.Sms
{
    /// <summary>
    /// Сервис Sms сообщений
    /// </summary>
    public interface ISmsService : IDisposable
    {
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="numberTo">Кому номер телефона</param>
        /// <param name="message">Сообщение</param>
        /// <returns></returns>
        Task<SmsResult> SendSmsAsync(string numberTo, string message);

        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="numberFrom">От кого номер телефона</param>
        /// <param name="numberTo">Кому номер телефона</param>
        /// <param name="message">Сообщение</param>
        /// <returns></returns>
        Task<SmsResult> SendSmsAsync(string numberFrom, string numberTo, string message);
    }
}
