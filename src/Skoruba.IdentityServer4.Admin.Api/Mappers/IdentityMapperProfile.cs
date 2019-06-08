using AutoMapper;
using Skoruba.IdentityServer4.Admin.Api.Dtos.Roles;
using Skoruba.IdentityServer4.Admin.Api.Dtos.Users;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.Api.Mappers
{
    public class IdentityMapperProfile<TRoleDto, TRoleDtoKey, TUserRolesDto, TUserDtoKey, TUserClaimsDto, TUserClaimDto, TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimDto, TRoleClaimsDto> : Profile
        where TUserClaimsDto : UserClaimsDto<TUserDtoKey>
        where TUserClaimDto : UserClaimDto<TUserDtoKey>
        where TRoleDto : RoleDto<TRoleDtoKey>
        where TUserRolesDto : UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey>
        where TUserProviderDto : UserProviderDto<TUserDtoKey>
        where TUserProvidersDto : UserProvidersDto<TUserDtoKey>
        where TUserChangePasswordDto : UserChangePasswordDto<TUserDtoKey>
        where TRoleClaimsDto : RoleClaimsDto<TRoleDtoKey>
        where TRoleClaimDto : RoleClaimDto<TRoleDtoKey>
    {
        public IdentityMapperProfile()
        {
            // entity to model
            CreateMap<TUserClaimsDto, UserClaimsApiDto<TUserDtoKey>>(MemberList.Destination);
            CreateMap<TUserClaimsDto, UserClaimApiDto<TUserDtoKey>>(MemberList.Destination);

            CreateMap<UserClaimApiDto<TUserDtoKey>, TUserClaimsDto>(MemberList.Source);
            CreateMap<TUserClaimDto, UserClaimApiDto<TUserDtoKey>>(MemberList.Destination);

            CreateMap<TUserRolesDto, UserRolesApiDto<TRoleDto>>(MemberList.Destination);
            CreateMap<UserRoleApiDto<TUserDtoKey, TRoleDtoKey>, TUserRolesDto>(MemberList.Destination);

            CreateMap<TUserProviderDto, UserProviderApiDto<TUserDtoKey>>(MemberList.Destination);
            CreateMap<TUserProvidersDto, UserProvidersApiDto<TUserDtoKey>>(MemberList.Destination);
            CreateMap<UserProviderDeleteApiDto<TUserDtoKey>, TUserProviderDto>(MemberList.Source);

            CreateMap<UserChangePasswordApiDto<TUserDtoKey>, TUserChangePasswordDto>(MemberList.Destination);

            CreateMap<RoleClaimsApiDto<TRoleDtoKey>, TRoleClaimsDto>(MemberList.Source);
            CreateMap<RoleClaimApiDto<TRoleDtoKey>, TRoleClaimDto>(MemberList.Destination);
            CreateMap<RoleClaimApiDto<TRoleDtoKey>, TRoleClaimsDto>(MemberList.Destination);
        }
    }
}