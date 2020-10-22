using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
	public class ApiResourceScopesDto
	{
		public ApiResourceScopesDto()
		{
			ApiResourceScopes = new List<ApiResourceScopeDto>();
		}
		
		public int ApiResourceId { get; set; }

		public string ApiResourceName { get; set; }

		public int PageSize { get; set; }

		public int TotalCount { get; set; }

		public List<ApiResourceScopeDto> ApiResourceScopes { get; set; }
	}
}