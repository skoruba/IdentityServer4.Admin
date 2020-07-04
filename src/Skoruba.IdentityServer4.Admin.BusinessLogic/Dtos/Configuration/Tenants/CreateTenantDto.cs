using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration.Tenants
{
    public class CreateTenantDto
    {
        public string Name { get; set; }

        public string EditionId { get; set; }

        public List<SelectItemDto> Editions { get; set; }
    }
}
