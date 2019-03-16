using System;

namespace Skoruba.IdentityServer4.Admin.Configuration.Interfaces
{
	[Obsolete]
	public interface IAdminConfiguration
    {
        string IdentityAdminRedirectUri { get; }

        string IdentityServerBaseUrl { get; }

        string IdentityAdminBaseUrl { get; }
    }
}