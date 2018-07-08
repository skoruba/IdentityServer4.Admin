using System;
using System.Xml.Linq;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Log
{
    public class LogDto
    {
        public long Id { get; set; }

        public string Message { get; set; }

        public string MessageTemplate { get; set; }

        public string Level { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public string Exception { get; set; }

        public string LogEvent { get; set; }

        public string Properties { get; set; }

        public XElement PropertiesXml => XElement.Parse(Properties);
    }
}
