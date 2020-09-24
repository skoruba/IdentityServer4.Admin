using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Helpers;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
	public class ClientSecretsDto
	{
		public ClientSecretsDto()
		{
			ClientSecrets = new List<ClientSecretDto>();
		}

		public int ClientId { get; set; }

	    public string ClientName { get; set; }

		public int ClientSecretId { get; set; }

        [Required]
		public string Type { get; set; } = "SharedSecret";

        public List<SelectItemDto> TypeList { get; set; }

        public string Description { get; set; }

        [Required]
		public string Value { get; set; }

		public string HashType { get; set; }

        public HashType HashTypeEnum
        {
            get
            {
                HashType result;

                if (Enum.TryParse(HashType, true, out result))
                {
                    return result;
                }

                return EntityFramework.Helpers.HashType.Sha256;
            }
        }

		public List<SelectItemDto> HashTypes { get; set; }

		public DateTime? Expiration { get; set; }

		public int TotalCount { get; set; }

		public int PageSize { get; set; }

		public List<ClientSecretDto> ClientSecrets { get; set; }
	}
}