// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using SkorubaIdentityServer4Admin.STS.Identity.Quickstart.Consent;

namespace IdentityServer4.Quickstart.UI.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string UserCode { get; set; }
    }
}