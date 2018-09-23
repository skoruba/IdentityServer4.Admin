using System;

namespace Skoruba.IdentityServer4.Admin.AspNetIdentity.ExceptionHandling
{
    public class UserFriendlyErrorPageIdentityException : Exception
    {
        public string ErrorKey { get; set; }
        
        public UserFriendlyErrorPageIdentityException(string message) : base(message)
        {
        }

        public UserFriendlyErrorPageIdentityException(string message, string errorKey) : base(message)
        {
            ErrorKey = errorKey;
        }

        public UserFriendlyErrorPageIdentityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
