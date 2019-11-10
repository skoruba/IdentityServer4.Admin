using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class CultureConfiguration
    {
        public static readonly string[] AvailableCultures = { "en", "fa", "fr", "ru", "sv", "zh" };
        public static readonly string DefaultRequestCulture = "en";

        public List<string> Cultures { get; set; }
        public string DefaultCulture { get; set; } = DefaultRequestCulture;
    }
}
