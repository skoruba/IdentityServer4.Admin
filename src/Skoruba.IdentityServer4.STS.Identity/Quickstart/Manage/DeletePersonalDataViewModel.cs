using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.STS.Identity.Quickstart.Manage
{
    public class DeletePersonalDataViewModel
    {
        public bool RequirePassword { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
    }
}
