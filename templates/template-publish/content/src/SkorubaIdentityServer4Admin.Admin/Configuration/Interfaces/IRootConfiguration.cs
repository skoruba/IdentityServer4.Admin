namespace SkorubaIdentityServer4Admin.Admin.Configuration.Interfaces
{
    public interface IRootConfiguration
    {
        AdminConfiguration AdminConfiguration { get; }
        IdentityDataConfiguration IdentityDataConfiguration { get; }
        IdentityServerDataConfiguration IdentityServerDataConfiguration { get; }
    }
}





