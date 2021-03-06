using System.Collections.Generic;

namespace SkorubaIdentityServer4Admin.Admin.Api.Dtos.ApiScopes
{
    public class ApiScopePropertiesApiDto
    {
        public ApiScopePropertiesApiDto()
        {
            ApiScopeProperties = new List<ApiScopePropertyApiDto>();
        }

        public List<ApiScopePropertyApiDto> ApiScopeProperties { get; set; } = new List<ApiScopePropertyApiDto>();

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}







