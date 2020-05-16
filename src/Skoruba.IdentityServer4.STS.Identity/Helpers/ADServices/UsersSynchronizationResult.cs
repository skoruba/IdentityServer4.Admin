using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers.ADServices
{
    public class UsersSynchronizationResult
    {
        public int NewUsersCount { get; set; }
        public int UpdatedUsersCount { get; set; }
        public int SyncErrorsCount { get; set; }
    }
}
