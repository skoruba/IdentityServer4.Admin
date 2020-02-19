using System;
using System.Threading.Tasks;

namespace Iserv.IdentityServer4.BusinessLogic.Sms
{
    /// <summary>
    /// Отправитель SMS сообщений
    /// </summary>
    public interface ISmsSender : IDisposable
    {
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="numberTo">Кому номер телефона</param>
        /// <param name="message">Сообщение</param>
        /// <returns></returns>
        Task<string> SendSmsAsync(string numberTo, string message);

        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="numberFrom">От кого номер телефона</param>
        /// <param name="numberTo">Кому номер телефона</param>
        /// <param name="message">Сообщение</param>
        /// <returns></returns>
        Task<string> SendSmsAsync(string numberFrom, string numberTo, string message);
    }
}
