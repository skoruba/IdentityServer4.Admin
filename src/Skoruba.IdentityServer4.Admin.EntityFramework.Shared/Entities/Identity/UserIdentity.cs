using Microsoft.AspNetCore.Identity;
using System;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity
{
    public class UserIdentity<TKey> : IdentityUser<TKey>
       where TKey : IEquatable<TKey>
    {
        public Guid Idext { get; set; }
    }

    public class UserIdentity : UserIdentity<string>
    {
        public UserIdentity()
        {
            Id = Guid.NewGuid().ToString();
        }
        public UserIdentity(string userName)
        {
            Id = Guid.NewGuid().ToString();
            UserName = userName;
        }
    }
}