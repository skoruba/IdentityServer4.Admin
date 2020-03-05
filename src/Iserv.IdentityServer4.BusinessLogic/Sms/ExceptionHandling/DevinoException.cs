using System;

namespace Iserv.IdentityServer4.BusinessLogic.Sms.ExceptionHandling
{
    public class DevinoException : Exception
    {
        public string ErrorKey { get; set; }

        public DevinoException(string message) : base(message)
        {
        }

        public DevinoException(string message, string errorKey) : base(message)
        {
            ErrorKey = errorKey;
        }

        public DevinoException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}