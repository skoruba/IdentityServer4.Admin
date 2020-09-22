namespace Skoruba.IdentityServer4.Admin.Api.Dtos.Users
{
    public class UserProviderApiDto<TKey>
    {
        public TKey UserId { get; set; }

        public string UserName { get; set; }

        public string ProviderKey { get; set; }

        public string LoginProvider { get; set; }

        public string ProviderDisplayName { get; set; }
    }
}