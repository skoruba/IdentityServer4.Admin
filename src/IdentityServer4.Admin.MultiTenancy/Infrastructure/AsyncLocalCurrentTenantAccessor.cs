using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace IdentityServer4.Admin.MultiTenancy.Infrastructure
{
    public class AsyncLocalCurrentTenantAccessor : ICurrentTenantAccessor
    {
        public BasicTenantInfo Current { get => _currentScope.Value; set => _currentScope.Value = value; }

        private readonly AsyncLocal<BasicTenantInfo> _currentScope;

        public AsyncLocalCurrentTenantAccessor()
        {
            _currentScope = new AsyncLocal<BasicTenantInfo>();
        }
    }
}
