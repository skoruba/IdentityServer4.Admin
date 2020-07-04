using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration.Tenants
{
    public class EditionDto
    {
        public EditionDto()
        {
            Tenants = new Collection<Tenant>();
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<Tenant> Tenants { get; set; }
    }
}
