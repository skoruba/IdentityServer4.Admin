using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using System;
using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration.Tenants
{
    public class TenantDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public EditionDto Edition { get; set; }

        public List<SelectItemDto> Editions { get; set; }
    }
}
