using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Entities
{
    public class Edition
    {
        public virtual Guid Id { get; protected set; }

        public virtual string Name { get; protected set; }

        public virtual ICollection<Tenant> Tenants { get; protected set; }

        public Edition(Guid id, string name)
        {
            Id = id;
            SetName(name);
            Tenants = new Collection<Tenant>();
        }

        public virtual void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }
            Name = name;
        }

        public virtual void AddTenant(Tenant tenant)
        {
            Tenants.Add(tenant);
        }

    }
}
