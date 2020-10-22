namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
	public class ApiResourceScopeDto
	{
		public int Id { get; set; }

		public string Scope { get; set; }

		public int ApiResourceId { get; set; }
	}
}