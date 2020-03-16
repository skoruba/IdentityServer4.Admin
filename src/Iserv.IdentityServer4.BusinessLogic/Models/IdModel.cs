using System;
using System.ComponentModel.DataAnnotations;

namespace Iserv.IdentityServer4.BusinessLogic.Models
{
    public class IdModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}