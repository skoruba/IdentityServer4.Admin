using System;

namespace Iserv.IdentityServer4.BusinessLogic.Sms.ExceptionHandling
{
    public class TwillioException : Exception
    {
        public string ErrorKey { get; set; }

        public TwillioException(string message) : base(message)
        {
        }

        public TwillioException(string message, string errorKey) : base(message)
        {
            ErrorKey = errorKey;
        }

        public TwillioException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}