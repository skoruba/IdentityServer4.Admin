using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources
{
    public interface ITenantServiceResources
    {
        ResourceMessage TenantDoesNotExist();
    }
}