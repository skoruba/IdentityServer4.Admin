using System.Collections.Generic;

namespace SkorubaIdentityServer4Admin.Admin.Api.Dtos.ApiResources
{
    public class ApiResourcesApiDto
    {
        public ApiResourcesApiDto()
        {
            ApiResources = new List<ApiResourceApiDto>();
        }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public List<ApiResourceApiDto> ApiResources { get; set; }
    }
}





