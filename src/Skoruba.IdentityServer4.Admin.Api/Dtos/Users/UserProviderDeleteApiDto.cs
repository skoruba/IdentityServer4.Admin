namespace Skoruba.IdentityServer4.Admin.Api.Dtos.Users
{
    public class UserProviderDeleteApiDto<TKey>
    {
        public TKey UserId { get; set; }

        public string ProviderKey { get; set; }

        public string LoginProvider { get; set; }
    }
}