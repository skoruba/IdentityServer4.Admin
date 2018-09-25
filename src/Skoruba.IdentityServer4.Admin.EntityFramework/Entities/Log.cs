using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Entities
{    
    public class Log
    {
        public long Id { get; set; }

        public string Message { get; set; }

        public string MessageTemplate { get; set; }
        
        public string Level { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public string Exception { get; set; }

        public string LogEvent { get; set; }
        
        public string Properties { get; set; }

        [NotMapped]
        public XElement PropertiesXml => XElement.Parse(Properties);
    }
}
