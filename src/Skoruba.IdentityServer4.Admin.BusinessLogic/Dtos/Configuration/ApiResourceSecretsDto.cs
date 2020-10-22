using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
    public class ApiResourceSecretsDto
    {
		public ApiResourceSecretsDto()
		{
            ApiResourceSecrets = new List<ApiResourceSecretDto>();
        }

        public int ApiSecretId { get; set; }

        public int ApiResourceId { get; set; }

        public string ApiResourceName { get; set; }

        [Required]
        public string Type { get; set; } = "SharedSecret";

        public List<SelectItemDto> TypeList { get; set; }

        public string Description { get; set; }

        [Required]
        public string Value { get; set; }

        public string HashType { get; set; }

        public List<SelectItemDto> HashTypes { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime? Expiration { get; set; }

        public List<ApiResourceSecretDto> ApiResourceSecrets { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}
