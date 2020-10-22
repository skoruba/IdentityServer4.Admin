namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
	public class ApiResourceClaimDto
	{
		public int Id { get; set; }

		public string Type { get; set; }

		public int ApiResourceId { get; set; }

		public virtual ApiResourceDto ApiResource { get; set; }
	}
}