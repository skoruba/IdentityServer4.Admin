using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Tenant
{
    public class TenantsDto
    {
        public TenantsDto()
        {
            Tenants = new List<TenantDto>();
        }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public List<TenantDto> Tenants { get; set; }
    }
}