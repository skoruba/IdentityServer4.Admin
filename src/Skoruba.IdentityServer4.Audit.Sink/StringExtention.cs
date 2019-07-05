using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Audit.Sink
{
    public static class StringExtension
    {
        public static string SafeForFormatted(this string value)
        {
            return value.Replace("{", "{{").Replace("}", "}}");
        }
    }
}