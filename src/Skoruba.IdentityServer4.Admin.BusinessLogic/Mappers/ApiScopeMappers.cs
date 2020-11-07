using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers
{
    public static class ApiScopeMappers
    {
        static ApiScopeMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiScopeMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static ApiScopesDto ToModel(this PagedList<ApiScope> scopes)
        {
            return scopes == null ? null : Mapper.Map<ApiScopesDto>(scopes);
        }

        public static ApiScopesDto ToModel(this ApiScope resource)
        {
            return resource == null ? null : Mapper.Map<ApiScopesDto>(resource);
        }

        public static ApiScope ToEntity(this ApiScopesDto resource)
        {
            return resource == null ? null : Mapper.Map<ApiScope>(resource);
        }
    }
}