using System;
using Bogus;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mocks
{
    public class IdentityDtoMock<TKey>
    {
        public static Faker<UserDto<TKey>> GetUserFaker(TKey id)
        {
            var userFaker = new Faker<UserDto<TKey>>()
                .RuleFor(o => o.Id, id)
                .RuleFor(o => o.UserName, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.Email, f => f.Internet.Email())
                .RuleFor(o => o.AccessFailedCount, f => f.Random.Int().ToString())
                .RuleFor(o => o.EmailConfirmed, f => f.Random.Bool())
                .RuleFor(o => o.PhoneNumberConfirmed, f => f.Random.Bool())
                .RuleFor(o => o.TwoFactorEnabled, f => f.Random.Bool())
                .RuleFor(o => o.LockoutEnd, f => f.Date.Future())
                .RuleFor(o => o.LockoutEnabled, f => true)
                .RuleFor(o => o.PhoneNumber, f => f.Random.Number().ToString());

            return userFaker;
        }

        public static Faker<RoleDto<TKey>> GetRoleFaker(TKey id)
        {
            var roleFaker = new Faker<RoleDto<TKey>>()
                .RuleFor(o => o.Id, id)
                .RuleFor(o => o.Name, f => Guid.NewGuid().ToString());

            return roleFaker;
        }

        public static Faker<UserChangePasswordDto<TKey>> GetUserChangePasswordFaker(TKey id, string password)
        {
            var roleFaker = new Faker<UserChangePasswordDto<TKey>>()
                .RuleFor(o => o.UserId, id)
                .RuleFor(o => o.Password, f => password)
                .RuleFor(o => o.ConfirmPassword, f => password);

            return roleFaker;
        }

        public static UserChangePasswordDto<TKey> GenerateRandomUserChangePassword(TKey id, string password)
        {
            var userChangePassword = GetUserChangePasswordFaker(id, password).Generate();

            return userChangePassword;
        }

        public static UserDto<TKey> GenerateRandomUser(TKey id)
        {
            var user = GetUserFaker(id).Generate();

            return user;
        }

        public static RoleDto<TKey> GenerateRandomRole(TKey id)
        {
            var role = GetRoleFaker(id).Generate();

            return role;
        }

        public static Faker<UserClaimsDto<TKey>> GetUserClaimsFaker(int id, TKey userId)
        {
            var userClaimFaker = new Faker<UserClaimsDto<TKey>>()
                .RuleFor(o => o.ClaimId, id)
                .RuleFor(o => o.ClaimType, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.ClaimValue, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.UserId, userId);

            return userClaimFaker;
        }

        public static UserClaimsDto<TKey> GenerateRandomUserClaim(int id, TKey userId)
        {
            var userClaim = GetUserClaimsFaker(id, userId).Generate();

            return userClaim;
        }

        public static Faker<UserRolesDto<TRoleDto, TKey, TKey>> GetUserRoleFaker<TRoleDto>(TKey id, TKey userId) 
            where TRoleDto : RoleDto<TKey>
        {
            var userRoleFaker = new Faker<UserRolesDto<TRoleDto, TKey, TKey>>()
                .RuleFor(o => o.RoleId, id)
                .RuleFor(o => o.UserId, userId);

            return userRoleFaker;
        }

        public static UserRolesDto<TRoleDto, TKey, TKey> GenerateRandomUserRole<TRoleDto>(TKey id, TKey userId) 
            where TRoleDto : RoleDto<TKey>
        {
            var userRole = GetUserRoleFaker<TRoleDto>(id, userId).Generate();

            return userRole;
        }

        public static Faker<UserProvidersDto<TKey>> GetUserProvidersFaker(string key, string loginProvider, TKey userId)
        {
            var userProvidersFaker = new Faker<UserProvidersDto<TKey>>()
                .RuleFor(o => o.LoginProvider, f => loginProvider)
                .RuleFor(o => o.ProviderKey, f => key)
                .RuleFor(o => o.ProviderDisplayName, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.UserId, userId);

            return userProvidersFaker;
        }

        public static UserProvidersDto<TKey> GenerateRandomUserProviders(string key, string loginProvider, TKey userId)
        {
            var provider = GetUserProvidersFaker(key, loginProvider, userId).Generate();

            return provider;
        }

        public static Faker<RoleClaimsDto<TKey>> GetRoleClaimFaker(int id, TKey roleId)
        {
            var roleClaimFaker = new Faker<RoleClaimsDto<TKey>>()
                .RuleFor(o => o.ClaimType, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.ClaimValue, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.RoleId, roleId)
                .RuleFor(o => o.ClaimId, id);

            return roleClaimFaker;
        }

        public static RoleClaimsDto<TKey> GenerateRandomRoleClaim(int id, TKey roleId)
        {
            var roleClaim = GetRoleClaimFaker(id, roleId).Generate();

            return roleClaim;
        }
    }
}
