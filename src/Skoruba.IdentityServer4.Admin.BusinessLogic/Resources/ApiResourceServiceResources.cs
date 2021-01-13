﻿using Skoruba.IdentityServer4.Admin.BusinessLogic.Helpers;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Resources
{
    public class ApiResourceServiceResources : IApiResourceServiceResources
    {
        public virtual ResourceMessage ApiResourceDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiResourceDoesNotExist),
                Description = ApiResourceServiceResource.ApiResourceDoesNotExist
            };
        }

        public virtual ResourceMessage ApiResourceExistsValue()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiResourceExistsValue),
                Description = ApiResourceServiceResource.ApiResourceExistsValue
            };
        }

        public virtual ResourceMessage ApiResourceExistsKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiResourceExistsKey),
                Description = ApiResourceServiceResource.ApiResourceExistsKey
            };
        }

        public virtual ResourceMessage ApiSecretDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiSecretDoesNotExist),
                Description = ApiResourceServiceResource.ApiSecretDoesNotExist
            };
        }

        public virtual ResourceMessage ApiResourcePropertyDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiResourcePropertyDoesNotExist),
                Description = ApiResourceServiceResource.ApiResourcePropertyDoesNotExist
            };
        }

        public virtual ResourceMessage ApiResourcePropertyExistsKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiResourcePropertyExistsKey),
                Description = ApiResourceServiceResource.ApiResourcePropertyExistsKey
            };
        }

        public virtual ResourceMessage ApiResourcePropertyExistsValue()
        {
            return new ResourceMessage()
            {
                Code = nameof(ApiResourcePropertyExistsValue),
                Description = ApiResourceServiceResource.ApiResourcePropertyExistsValue
            };
        }
    }
}
