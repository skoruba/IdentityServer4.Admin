using System.Collections.Generic;

namespace Skoruba.MultiTenant.Finbuckle.Configuration
{

    public class Configuration
    {
        public List<RequestParam> RequestParameters { get; set; } = new List<RequestParam>();
        public string TenantKey { get; set; } = "SkorubaTenant";
    }
}