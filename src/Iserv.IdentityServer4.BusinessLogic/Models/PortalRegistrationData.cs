namespace Iserv.IdentityServer4.BusinessLogic.Models
{
    /// <summary>
    /// Данные для заполнения профиля
    /// при регистрации на портале
    /// </summary>
    public class PortalRegistrationData
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Password { get; set; }
    }
}
