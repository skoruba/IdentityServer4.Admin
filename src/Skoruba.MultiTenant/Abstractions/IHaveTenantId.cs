namespace Skoruba.MultiTenant.Abstractions
{
    public interface IHaveTenantId
    {
        public string TenantId { get; set; }
    }
}
