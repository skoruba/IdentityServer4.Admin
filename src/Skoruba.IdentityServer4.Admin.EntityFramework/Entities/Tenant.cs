using System;
using System.Collections.Generic;
using System.Linq;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Entities
{
    public class Tenant
    {
        public virtual Guid Id { get; protected set; }

        public virtual string Name { get; protected set; }

        public virtual DateTime CreatedTime { get; set; }

        public virtual DateTime LastUpdatedTime { get; set; }

        public virtual List<TenantConnectionString> ConnectionStrings { get; protected set; }

        protected Tenant()
        {

        }

        public Tenant(Guid id, string name)
        {
            Id = id;
            SetName(name);
            CreatedTime = DateTime.UtcNow;
            ConnectionStrings = new List<TenantConnectionString>();
        }

        public virtual void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }
            Name = name;
        }

        public virtual string FindConnectionString(string name)
        {
            return ConnectionStrings.FirstOrDefault(c => c.Name == name)?.Value;
        }

        public virtual void SetConnectionString(string name, string connectionString)
        {
            var tenantConnectionString = ConnectionStrings.FirstOrDefault(x => x.Name == name);

            if (tenantConnectionString != null)
            {
                tenantConnectionString.SetValue(connectionString);
            }
            else
            {
                ConnectionStrings.Add(new TenantConnectionString(Id, name, connectionString));
            }
        }

        public virtual void RemoveConnectionString(string name)
        {
            var tenantConnectionString = ConnectionStrings.FirstOrDefault(x => x.Name == name);

            if (tenantConnectionString != null)
            {
                ConnectionStrings.Remove(tenantConnectionString);
            }
        }
    }
}
