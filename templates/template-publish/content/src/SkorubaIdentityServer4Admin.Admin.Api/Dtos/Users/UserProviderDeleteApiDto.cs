namespace SkorubaIdentityServer4Admin.Admin.Api.Dtos.Users
{
    public class UserProviderDeleteApiDto<TKey>
    {
        public TKey UserId { get; set; }

        public string ProviderKey { get; set; }

        public string LoginProvider { get; set; }
    }
}





