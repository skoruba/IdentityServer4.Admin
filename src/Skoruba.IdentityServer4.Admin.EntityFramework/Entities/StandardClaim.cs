using System;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Entities
{
    public class StandardClaim
    {
        public int Id { get; set; }

        public string ClaimType { get; set; }

        public DateTimeOffset? LastUsedTimestamp { get; set; }

        public long UseCount { get; set; }
    }
}
