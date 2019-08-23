using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.ViewModels.Manage
{
    [Serializable]
    public class StatusViewModel
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; }

        public List<string> ErrorsDetails { get; set; } = new List<string>();
    }
}
