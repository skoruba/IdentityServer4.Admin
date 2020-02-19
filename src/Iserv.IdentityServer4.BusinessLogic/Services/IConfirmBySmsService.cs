using System.Threading.Tasks;

namespace Iserv.IdentityServer4.BusinessLogic.Services
{
    /// <summary>
    /// Сервис подтверждения по смс
    /// </summary>    
    public interface IConfirmBySmsService
    {
        /// <summary>
        /// Запрос на подтверждение действия по смс
        /// </summary>
        /// <param name="phone">Номер телефона отправления кода подтверждения</param>
        /// <param name="actionName">Наименование действия</param>
        /// <param name="templateMessage">Шаблон сообщения</param>
        /// <returns>Результат запроса на подтверждение действия по смс</returns>
        Task ConfirmSmsAsync(string phone, string actionName, string templateMessage);

        /// <summary>
        /// Проверка кода подтверждения действия по смс
        /// </summary>
        /// <param name="phone">Номер телефона проверки кода подтверждения</param>
        /// <param name="actionName">Наименование действия</param>
        /// <param name="code">Код смс подтверждения действия</param>
        /// <returns>Результат проверки кода подтверждения действия</returns>
        void ValidSmsCode(string phone, string actionName, string code);
    }
}