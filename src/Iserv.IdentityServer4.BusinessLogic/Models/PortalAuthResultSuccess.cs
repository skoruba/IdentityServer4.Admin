using System;

namespace Iserv.IdentityServer4.BusinessLogic.Models
{
    /// <summary>
    /// Успешный результат запроса регистрации/авторизации профиля на портале
    /// </summary>
    public class PortalAuthResultSuccess
    {
        /// <summary>
        /// Внешний идентификатор профиля портала
        /// </summary>
        public Guid UserId { get; set; }
    }
}
