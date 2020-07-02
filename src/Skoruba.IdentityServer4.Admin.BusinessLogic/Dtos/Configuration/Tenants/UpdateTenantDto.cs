using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration.Tenants
{
    public class UpdateTenantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
