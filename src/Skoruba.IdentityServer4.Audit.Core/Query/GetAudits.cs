using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Audit.Core.Query
{
    public class GetAudits
    {
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public string SubjectId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}