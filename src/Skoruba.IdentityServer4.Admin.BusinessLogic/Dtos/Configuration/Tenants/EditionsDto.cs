using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration.Tenants
{
	public class EditionsDto
	{
		public EditionsDto()
		{
			Editions = new List<EditionDto>();
		}

		public int PageSize { get; set; }

		public int TotalCount { get; set; }

		public List<EditionDto> Editions { get; set; }
	}
}
