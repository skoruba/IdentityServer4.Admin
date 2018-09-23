using Skoruba.IdentityServer4.Admin.AspNetIdentity.Dtos.Identity.Base;

namespace Skoruba.IdentityServer4.Admin.AspNetIdentity.Dtos.Identity
{
    public class UserProviderDto<TUserDtoKey> : BaseUserProviderDto<TUserDtoKey>
    {
        public string ProviderKey { get; set; }

        public string LoginProvider { get; set; }

        public string ProviderDisplayName { get; set; }        
    }
}
