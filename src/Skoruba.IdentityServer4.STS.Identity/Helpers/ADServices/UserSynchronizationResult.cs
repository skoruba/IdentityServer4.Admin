using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers.ADServices
{
    public class UserSynchronizationResult<TUser>
    {
        public bool Created { get; set; }
        public bool Updated { get; set; }
        public bool Error { get; set; }
        public TUser User { get; set; }

    }
}
