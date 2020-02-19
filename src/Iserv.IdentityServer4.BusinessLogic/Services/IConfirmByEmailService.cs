using System.Threading.Tasks;

namespace Iserv.IdentityServer4.BusinessLogic.Services
{
    /// <summary>
    /// Сервис подтверждения по email
    /// </summary>    
    public interface IConfirmByEmailService
    {
        /// <summary>
        /// Запрос на подтверждение действия по email
        /// </summary>
        /// <param name="email">Email отправления кода подтверждения</param>
        /// <param name="actionName">Наименование действия</param>
        /// <param name="title">Заголовок письма</param>
        /// <param name="templateMessage">Шаблон сообщения</param>
        /// <returns>Результат запроса на подтверждение действия по смс</returns>
        Task ConfirmEmailAsync(string email, string actionName, string title, string templateMessage);

        /// <summary>
        /// Проверка кода подтверждения действия по email
        /// </summary>
        /// <param name="email">Email проверки кода подтверждения</param>
        /// <param name="actionName">Наименование действия</param>
        /// <param name="code">Код смс подтверждения действия</param>
        /// <returns>Результат проверки кода подтверждения действия</returns>
        void ValidEmailCode(string email, string actionName, string code);
    }
}