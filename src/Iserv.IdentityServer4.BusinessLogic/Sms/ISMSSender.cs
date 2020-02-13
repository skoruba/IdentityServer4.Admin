using System;
using System.Threading.Tasks;

namespace Iserv.IdentityServer4.BusinessLogic.Sms
{
    /// <summary>
    /// Отправитель SMS сообщений
    /// </summary>
    public interface ISMSSender : IDisposable
    {
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="numberTo">Кому номер телефона</param>
        /// <param name="message">Сообщение</param>
        /// <returns></returns>
        Task<string> SendSMSAsync(string numberTo, string message);

        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="numberTo">От кого номер телефона</param>
        /// <param name="numberTo">Кому номер телефона</param>
        /// <param name="message">Сообщение</param>
        /// <returns></returns>
        Task<string> SendSMSAsync(string numberFrom, string numberTo, string message);
    }
}
