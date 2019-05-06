using AutoMapper;

namespace Skoruba.IdentityServer4.Admin.Api.Mappers
{
    public static class ClientApiMappers
    {
        static ClientApiMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientApiMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }
        
        public static T ToApiModel<T>(this object source)
        {
            return Mapper.Map<T>(source);
        }
    }
}