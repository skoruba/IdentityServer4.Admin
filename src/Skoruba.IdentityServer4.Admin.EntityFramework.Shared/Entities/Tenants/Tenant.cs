using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Tenants
{
    public class Tenant
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string DomainName { get; set; }

        public string DatabaseName { get; set; }
        public bool RequireTwoFactorAuthentication { get; set; }

        [Required]
        [MaxLength(4)]
        [MinLength(4)]
        public string Code { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}