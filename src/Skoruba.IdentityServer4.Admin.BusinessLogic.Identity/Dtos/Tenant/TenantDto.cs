using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Tenant
{
    public class TenantDto
    {
        public bool IsDefaultId => EqualityComparer<string>.Default.Equals(Id, Guid.Empty.ToString());
        public string Id { get; set; }
        public string Name { get; set; }
        public string DomainName { get; set; }
        public string DataBaseName { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
    }
}