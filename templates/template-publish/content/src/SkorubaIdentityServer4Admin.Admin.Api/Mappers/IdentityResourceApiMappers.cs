using AutoMapper;

namespace SkorubaIdentityServer4Admin.Admin.Api.Mappers
{
    public static class IdentityResourceApiMappers
    {
        static IdentityResourceApiMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<IdentityResourceApiMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static T ToIdentityResourceApiModel<T>(this object source)
        {
            return Mapper.Map<T>(source);
        }
    }
}





