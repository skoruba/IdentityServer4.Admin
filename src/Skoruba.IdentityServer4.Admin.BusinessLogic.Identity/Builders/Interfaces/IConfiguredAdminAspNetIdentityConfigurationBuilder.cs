using System;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Builders.Interfaces
{
    public interface IConfiguredAdminAspNetIdentityConfigurationBuilder<TKey>
        where TKey : IEquatable<TKey>
    {
        AdminAspNetIdentityConfiguration<TKey> Build();
    }
}