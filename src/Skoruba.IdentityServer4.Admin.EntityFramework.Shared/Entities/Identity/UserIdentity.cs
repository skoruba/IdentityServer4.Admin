using Microsoft.AspNetCore.Identity;
using Skoruba.MultiTenant.Abstractions;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity
{
    public class UserIdentity : IdentityUser, IHaveTenantId
	{
		public string TenantId { get; set; }
	}
}