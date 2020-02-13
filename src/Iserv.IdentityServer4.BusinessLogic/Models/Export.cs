using IdentityServer4.EntityFramework.Entities;

namespace Iserv.IdentityServer4.BusinessLogic.Models
{
    public class Export
    {
        public Client[] Clients { get; set; }
        public IdentityResource[] IdentityResources { get; set; }
        public ApiResource[] ApiResources { get; set; }
    }
}