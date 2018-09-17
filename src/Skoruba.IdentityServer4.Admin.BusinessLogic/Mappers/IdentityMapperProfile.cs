// Based on the IdentityServer4.EntityFramework - authors - Brock Allen & Dominick Baier.
// https://github.com/IdentityServer/IdentityServer4.EntityFramework

// Modified by Jan Škoruba

using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Common;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers
{
    public class IdentityMapperProfile<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : Profile
        where TUserDto : UserDto<TUserDtoKey>
        where TRoleDto : RoleDto<TRoleDtoKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
    {
        public IdentityMapperProfile()
        {
            // entity to model
            CreateMap<TUser, TUserDto>(MemberList.Destination);
            
            CreateMap<UserLoginInfo, UserProviderDto<TUserDtoKey>>(MemberList.Destination);

            CreateMap<IdentityError, ViewErrorMessage>(MemberList.Destination)
                .ForMember(x => x.ErrorKey, opt => opt.MapFrom(src => src.Code))
                .ForMember(x => x.ErrorMessage, opt => opt.MapFrom(src => src.Description));

            // entity to model
            CreateMap<TRole, TRoleDto>(MemberList.Destination);

            CreateMap<TUser, TUser>(MemberList.Destination)
                .ForMember(x => x.SecurityStamp, opt => opt.Ignore());

            CreateMap<PagedList<TUser>, UsersDto<TUserDto, TUserDtoKey>>(MemberList.Destination)
                .ForMember(x => x.Users,
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<TUserClaim, UserClaimDto<TUserDtoKey>>(MemberList.Destination)
                .ForMember(x => x.ClaimId, opt => opt.MapFrom(src => src.Id));

            CreateMap<TUserClaim, UserClaimsDto<TUserDtoKey>>(MemberList.Destination)
                .ForMember(x => x.ClaimId, opt => opt.MapFrom(src => src.Id));

            CreateMap<PagedList<TRole>, RolesDto<TRoleDto, TRoleDtoKey>>(MemberList.Destination)
                .ForMember(x => x.Roles,
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<PagedList<UserIdentityRole>, UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey>>(MemberList.Destination)
                .ForMember(x => x.Roles,
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<PagedList<TUserClaim>, UserClaimsDto<TUserDtoKey>>(MemberList.Destination)
                .ForMember(x => x.Claims,
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<PagedList<TRoleClaim>, RoleClaimsDto<TRoleDtoKey>>(MemberList.Destination)
                .ForMember(x => x.Claims,
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<List<UserLoginInfo>, UserProvidersDto<TUserDtoKey>>(MemberList.Destination)
                .ForMember(x => x.Providers, opt => opt.MapFrom(src => src));

            CreateMap<TRoleClaim, RoleClaimDto<TRoleDtoKey>>(MemberList.Destination)
                .ForMember(x => x.ClaimId, opt => opt.MapFrom(src => src.Id));

            CreateMap<TRoleClaim, RoleClaimsDto<TRoleDtoKey>>(MemberList.Destination)
                .ForMember(x => x.ClaimId, opt => opt.MapFrom(src => src.Id));

            CreateMap<UserIdentityUserLogin, UserProviderDto<TUserDtoKey>>(MemberList.Destination);

            // model to entity
            CreateMap<TRoleDto, TRole>(MemberList.Source);

            CreateMap<RoleClaimsDto<TRoleDtoKey>, TRoleClaim>(MemberList.Source);

            CreateMap<UserClaimsDto<TUserDtoKey>, TUserClaim>(MemberList.Source)
                .ForMember(x => x.Id,
                    opt => opt.MapFrom(src => src.ClaimId));

            // model to entity
            CreateMap<TUserDto, TUser>(MemberList.Source);
        }
    }
}