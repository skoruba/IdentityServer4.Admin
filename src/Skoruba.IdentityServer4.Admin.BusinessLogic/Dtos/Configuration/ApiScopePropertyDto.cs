namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration
{
    public class ApiScopePropertyDto
    {
        public int Id { get; set; }

        public int ScopeId { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }

        public virtual ApiScopeDto ApiScope { get; set; }
    }
}