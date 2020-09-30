namespace Skoruba.IdentityServer4.Shared.Configuration.Email
{
    public class SendGridConfiguration
    {
        public string ApiKey { get; set; }
        public string SourceEmail { get; set; }
        public string SourceName { get; set; }
        public bool EnableClickTracking { get; set; } = false;
    }
}