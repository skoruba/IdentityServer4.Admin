using System;
using Bogus;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mocks
{
    public class IdentityDtoMock
    {
        public static Faker<UserDto> GetUserFaker(int id)
        {
            var userFaker = new Faker<UserDto>()
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

        public static Faker<RoleDto> GetRoleFaker(int id)
        {
            var roleFaker = new Faker<RoleDto>()
                .RuleFor(o => o.Id, id)
                .RuleFor(o => o.Name, f => Guid.NewGuid().ToString());

            return roleFaker;
        }

        public static Faker<UserChangePasswordDto> GetUserChangePasswordFaker(int id, string password)
        {
            var roleFaker = new Faker<UserChangePasswordDto>()
                .RuleFor(o => o.UserId, id)
                .RuleFor(o => o.Password, f => password)
                .RuleFor(o => o.ConfirmPassword, f => password);

            return roleFaker;
        }

        public static UserChangePasswordDto GenerateRandomUserChangePassword(int id, string password)
        {
            var userChangePassword = GetUserChangePasswordFaker(id, password).Generate();

            return userChangePassword;
        }

        public static UserDto GenerateRandomUser(int id)
        {
            var user = GetUserFaker(id).Generate();

            return user;
        }

        public static RoleDto GenerateRandomRole(int id)
        {
            var role = GetRoleFaker(id).Generate();

            return role;
        }

        public static Faker<UserClaimsDto> GetUserClaimsFaker(int id, int userId)
        {
            var userClaimFaker = new Faker<UserClaimsDto>()
                .RuleFor(o => o.ClaimId, id)
                .RuleFor(o => o.ClaimType, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.ClaimValue, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.UserId, userId);

            return userClaimFaker;
        }

        public static UserClaimsDto GenerateRandomUserClaim(int id, int userId)
        {
            var userClaim = GetUserClaimsFaker(id, userId).Generate();

            return userClaim;
        }

        public static Faker<UserRolesDto> GetUserRoleFaker(int id, int userId)
        {
            var userRoleFaker = new Faker<UserRolesDto>()
                .RuleFor(o => o.RoleId, id)
                .RuleFor(o => o.UserId, userId);

            return userRoleFaker;
        }

        public static UserRolesDto GenerateRandomUserRole(int id, int userId)
        {
            var userRole = GetUserRoleFaker(id, userId).Generate();

            return userRole;
        }

        public static Faker<UserProvidersDto> GetUserProvidersFaker(string key, string loginProvider, int userId)
        {
            var userProvidersFaker = new Faker<UserProvidersDto>()
                .RuleFor(o => o.LoginProvider, f => loginProvider)
                .RuleFor(o => o.ProviderKey, f => key)
                .RuleFor(o => o.ProviderDisplayName, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.UserId, userId);

            return userProvidersFaker;
        }

        public static UserProvidersDto GenerateRandomUserProviders(string key, string loginProvider, int userId)
        {
            var provider = GetUserProvidersFaker(key, loginProvider, userId).Generate();

            return provider;
        }

        public static Faker<RoleClaimsDto> GetRoleClaimFaker(int id, int roleId)
        {
            var roleClaimFaker = new Faker<RoleClaimsDto>()
                .RuleFor(o => o.ClaimType, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.ClaimValue, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.RoleId, roleId)
                .RuleFor(o => o.ClaimId, id);

            return roleClaimFaker;
        }

        public static RoleClaimsDto GenerateRandomRoleClaim(int id, int roleId)
        {
            var roleClaim = GetRoleClaimFaker(id, roleId).Generate();

            return roleClaim;
        }
    }
}
