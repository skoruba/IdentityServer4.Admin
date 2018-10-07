using Microsoft.AspNetCore.Identity;

namespace SkorubaIdentityServer4Admin.Admin.Helpers.Identity
{
    /// <inheritdoc />
    /// <summary>
    /// If you want to create localization for Asp.Net Identity - it is one way how do that - override methods here
    /// And register this class into DI system - services.AddTransient - IdentityErrorDescriber, IdentityErrorMessages
    /// Or install package with specific language - https://www.nuget.org/packages?q=Microsoft.AspNet.Identity.Core
    /// </summary>
    public class IdentityErrorMessages : IdentityErrorDescriber
    {        

    }
}
