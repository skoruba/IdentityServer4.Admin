using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
    public class ApiScopePropertiesDto
    {
        public int ApiScopePropertyId { get; set; }

        public int ApiScopeId { get; set; }

        public string ApiScopeName { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

        public List<ApiScopePropertyDto> ApiScopeProperties { get; set; } = new List<ApiScopePropertyDto>();

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}