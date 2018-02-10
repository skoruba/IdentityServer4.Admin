using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using Skoruba.IdentityServer4.Admin.Constants;

namespace Skoruba.IdentityServer4.Admin.Data.Entities
{
    [Table(ConfigurationConsts.LoggingTableName)]
    public class Log
    {
        [Key]
        public long Id { get; set; }

        public string Message { get; set; }

        public string MessageTemplate { get; set; }

        [MaxLength(128)]
        public string Level { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public string Exception { get; set; }

        public string LogEvent { get; set; }

        [Column(TypeName = "xml")]
        public string Properties { get; set; }

        [NotMapped]
        public XElement PropertiesXml => XElement.Parse(Properties);
    }
}
