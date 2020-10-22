namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
	public class ApiScopeClaimsDto
	{
		public int Id { get; set; }

		public string Type { get; set; }

		public int ScopeId { get; set; }

		public virtual ApiScopeDto ApiScope { get; set; }
	}
}