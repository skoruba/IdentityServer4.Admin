using IdentityServer4.EntityFramework.Entities;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Models
{
    public class Export
    {
        public Client[] Clients { get; set; }
        public IdentityResource[] IdentityResources { get; set; }
        public ApiResource[] ApiResources { get; set; }
    }
}