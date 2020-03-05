namespace Iserv.IdentityServer4.BusinessLogic.Sms
{
    public class SmsResult
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
    }

    public class SmsResult<T> : SmsResult
    {
        public T Value { get; set; }
    }
}
