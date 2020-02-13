namespace Iserv.IdentityServer4.BusinessLogic.Models
{
    public class PortalResult
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
    }

    public class PortalResult<T> : PortalResult
    {
        public T Value { get; set; }
    }
}
