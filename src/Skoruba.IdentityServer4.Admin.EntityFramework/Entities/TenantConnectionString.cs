using System;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Entities
{
    public class TenantConnectionString
    {
        public virtual Guid TenantId { get; protected set; }

        public virtual string Name { get; protected set; }

        public virtual string Value { get; protected set; }

        protected TenantConnectionString()
        {

        }

        public TenantConnectionString(Guid tenantId, string name, string value)
        {
            TenantId = tenantId;
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }
            Name = name;
            SetValue(value);
        }

        public virtual void SetValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(nameof(value));
            }
            Value = value;
        }
    }
}
