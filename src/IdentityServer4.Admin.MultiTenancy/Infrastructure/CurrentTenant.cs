using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.Admin.MultiTenancy.Infrastructure
{
    public class CurrentTenant : ICurrentTenant
    {
        public virtual bool isAvailable => Id.HasValue;

        public virtual Guid? Id => _currentTenantAccessor.Current?.TenantId;

        public string Name => _currentTenantAccessor.Current?.Name;

        private readonly ICurrentTenantAccessor _currentTenantAccessor;

        public CurrentTenant(ICurrentTenantAccessor currentTenantAccessor)
        {
            _currentTenantAccessor = currentTenantAccessor;
        }

        public IDisposable Change(Guid? id, string name = null)
        {
            return SetCurrent(id, name);
        }

        private IDisposable SetCurrent(Guid? tenantId, string name = null)
        {
            var previous = _currentTenantAccessor.Current;
            _currentTenantAccessor.Current = new BasicTenantInfo(tenantId, name);
            return new DisposeAction(() =>
            {
                _currentTenantAccessor.Current = previous;
            });
        }
    }
}
