using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.Configuration
{
    public class CultureConfiguration
    {
        public static readonly string[] AvailableCultures = { "en", "fa", "fr", "ru", "sv", "zh", "es", "da", "de", "fi" };
        public static readonly string DefaultRequestCulture = "en";

        public List<string> Cultures { get; set; }
        public string DefaultCulture { get; set; } = DefaultRequestCulture;
    }
}
