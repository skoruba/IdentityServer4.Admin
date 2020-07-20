using IdentityServer4.Admin.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using System;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Entitites.Identity
{
	public class UserIdentityRole : UserIdentityRole<string>, IMultiTenant
	{
	}

	public class UserIdentityRole<TKey> : IdentityRole<TKey>, IMultiTenant
		where TKey : IEquatable<TKey>
	{
		public Guid? TenantId { get; protected set; }
	}
}