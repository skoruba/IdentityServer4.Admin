using System;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
    public class ClientSecretDto
    {
        [Required]
        public string Type { get; set; } = "SharedSecret";

		public int Id { get; set; }

		public string Description { get; set; }

        [Required]
		public string Value { get; set; }

		public DateTime? Expiration { get; set; }

        public DateTime Created { get; set; }
	}
}
