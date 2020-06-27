using System;

namespace IdentityServer4.Admin.MultiTenancy.Infrastructure
{
    public interface ICurrentTenant
    {
        bool isAvailable { get; }
        Guid? Id { get; }
        string Name { get; }
        IDisposable Change(Guid? id, string name = null);
    }
}
