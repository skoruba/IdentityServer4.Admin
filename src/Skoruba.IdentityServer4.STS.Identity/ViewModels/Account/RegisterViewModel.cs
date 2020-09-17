using System;
using System.Collections.Generic;
using System.Linq;

namespace Skoruba.IdentityServer4.STS.Identity.ViewModels.Account
{
    public class RegisterViewModel: RegisterInputModel
    {
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !String.IsNullOrWhiteSpace(x.DisplayName));

    }
}