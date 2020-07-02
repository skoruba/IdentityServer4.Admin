using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration.Tenants
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
