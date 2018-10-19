using System;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling
{
    public class UserFriendlyErrorPageException : Exception
    {
        public string ErrorKey { get; set; }
        
        public UserFriendlyErrorPageException(string message) : base(message)
        {
        }

        public UserFriendlyErrorPageException(string message, string errorKey) : base(message)
        {
            ErrorKey = errorKey;
        }

        public UserFriendlyErrorPageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
