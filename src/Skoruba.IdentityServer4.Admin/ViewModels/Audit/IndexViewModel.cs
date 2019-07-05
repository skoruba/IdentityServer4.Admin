using Skoruba.IdentityServer4.Audit.Core.Dtos;
using Skoruba.IdentityServer4.Audit.Core.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.ViewModels.Audit
{
    public class IndexViewModel
    {
        public AuditsDto Audits { get; set; }
        public GetAudits GetAudits { get; set; }
    }
}