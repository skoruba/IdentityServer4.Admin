using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers
{
    public static class ApiScopeMappers
    {
        static ApiScopeMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiResourceMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }


        public static ApiScopesDto ToModel(this PagedList<ApiScope> scopes)
        {
            return scopes == null ? null : Mapper.Map<ApiScopesDto>(scopes);
        }

        public static ApiScopesDto ToModel(this ApiScope apiScope)
        {
            return apiScope == null ? null : Mapper.Map<ApiScopesDto>(apiScope);
        }

        public static ApiScope ToEntity(this ApiScopesDto resource)
        {
            return resource == null ? null : Mapper.Map<ApiScope>(resource);
        }

        public static ApiScope ToEntity(this ApiScopeDto resource)
        {
            return resource == null ? null : Mapper.Map<ApiScope>(resource);
        }

    }
}