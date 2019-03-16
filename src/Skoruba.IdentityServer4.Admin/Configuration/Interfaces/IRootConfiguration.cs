using System;

namespace Skoruba.IdentityServer4.Admin.Configuration.Interfaces
{
	[Obsolete]
	public interface IRootConfiguration
    {
        IAdminConfiguration AdminConfiguration { get; }
    }
}