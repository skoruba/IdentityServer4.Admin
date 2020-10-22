using System;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
	public class ApiSecretDto
	{
		public int Id { get; set; }

		public string Description { get; set; }

	    [Required]
        public string Value { get; set; }

		public DateTime? Expiration { get; set; }
		
		[Required]
        public string Type { get; set; } = "SharedSecret";

        public DateTime Created { get; set; }

		public int ApiResourceId { get; set; }

		public virtual ApiResourceDto ApiResource { get; set; }
	}
}