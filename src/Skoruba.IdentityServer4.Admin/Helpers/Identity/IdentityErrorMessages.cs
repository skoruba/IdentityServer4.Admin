using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Skoruba.IdentityServer4.Admin.Helpers.Identity
{
    /// <inheritdoc />
    /// <summary>
    /// If you want to create localization for Asp.Net Identity - it is one way how do that - override methods here
    /// And register this class into DI system - services.AddTransient - IdentityErrorDescriber, IdentityErrorMessages
    /// Or install package with specific language - https://www.nuget.org/packages?q=Microsoft.AspNet.Identity.Core
    /// </summary>
    public class IdentityErrorMessages : IdentityErrorDescriber
    {
        private readonly IStringLocalizer _stringLocalizer;

        public IdentityErrorMessages(IStringLocalizer<IdentityErrorMessages> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a concurrency failure.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating a concurrency failure.</returns>
        public override IdentityError ConcurrencyFailure()
        {
            return CreateNamedError(nameof(ConcurrencyFailure));
        }

        /// <summary>
        /// Returns the default <see cref="IdentityError" />.
        /// </summary>
        /// <returns>The default <see cref="IdentityError" />.</returns>
        public override IdentityError DefaultError()
        {
            return CreateNamedError(nameof(DefaultError));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating the specified <paramref name="email" /> is already associated with an account.
        /// </summary>
        /// <param name="email">The email that is already associated with an account.</param>
        /// <returns>An <see cref="IdentityError" /> indicating the specified <paramref name="email" /> is already associated with an account.</returns>
        public override IdentityError DuplicateEmail(string email)
        {
            return CreateNamedError(nameof(DuplicateEmail));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating the specified <paramref name="role" /> name already exists.
        /// </summary>
        /// <param name="role">The duplicate role.</param>
        /// <returns>An <see cref="IdentityError" /> indicating the specific role <paramref name="role" /> name already exists.</returns>
        public override IdentityError DuplicateRoleName(string role)
        {
            return CreateNamedError(nameof(DuplicateRoleName));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating the specified <paramref name="userName" /> already exists.
        /// </summary>
        /// <param name="userName">The user name that already exists.</param>
        /// <returns>An <see cref="IdentityError" /> indicating the specified <paramref name="userName" /> already exists.</returns>
        public override IdentityError DuplicateUserName(string userName)
        {
            return CreateNamedError(nameof(DuplicateUserName));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating the specified <paramref name="email" /> is invalid.
        /// </summary>
        /// <param name="email">The email that is invalid.</param>
        /// <returns>An <see cref="IdentityError" /> indicating the specified <paramref name="email" /> is invalid.</returns>
        public override IdentityError InvalidEmail(string email)
        {
            return CreateNamedError(nameof(InvalidEmail));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating the specified <paramref name="role" /> name is invalid.
        /// </summary>
        /// <param name="role">The invalid role.</param>
        /// <returns>An <see cref="IdentityError" /> indicating the specific role <paramref name="role" /> name is invalid.</returns>
        public override IdentityError InvalidRoleName(string role)
        {
            return CreateNamedError(nameof(InvalidRoleName));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating an invalid token.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating an invalid token.</returns>
        public override IdentityError InvalidToken()
        {
            return CreateNamedError(nameof(InvalidToken));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating the specified user <paramref name="userName" /> is invalid.
        /// </summary>
        /// <param name="userName">The user name that is invalid.</param>
        /// <returns>An <see cref="IdentityError" /> indicating the specified user <paramref name="userName" /> is invalid.</returns>
        public override IdentityError InvalidUserName(string userName)
        {
            return CreateNamedError(nameof(InvalidUserName));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating an external login is already associated with an account.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating an external login is already associated with an account.</returns>
        public override IdentityError LoginAlreadyAssociated()
        {
            return CreateNamedError(nameof(LoginAlreadyAssociated));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a password mismatch.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating a password mismatch.</returns>
        public override IdentityError PasswordMismatch()
        {
            return CreateNamedError(nameof(PasswordMismatch));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a password entered does not contain a numeric character, which is required by the password policy.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating a password entered does not contain a numeric character.</returns>
        public override IdentityError PasswordRequiresDigit()
        {
            return CreateNamedError(nameof(PasswordRequiresDigit));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a password entered does not contain a lower case letter, which is required by the password policy.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating a password entered does not contain a lower case letter.</returns>
        public override IdentityError PasswordRequiresLower()
        {
            return CreateNamedError(nameof(PasswordRequiresLower));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a password entered does not contain a non-alphanumeric character, which is required by the password policy.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating a password entered does not contain a non-alphanumeric character.</returns>
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return CreateNamedError(nameof(PasswordRequiresNonAlphanumeric));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a password does not meet the minimum number <paramref name="uniqueChars" /> of unique chars.
        /// </summary>
        /// <param name="uniqueChars">The number of different chars that must be used.</param>
        /// <returns>An <see cref="IdentityError" /> indicating a password does not meet the minimum number <paramref name="uniqueChars" /> of unique chars.</returns>
        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return CreateNamedError(nameof(PasswordRequiresUniqueChars));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a password entered does not contain an upper case letter, which is required by the password policy.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating a password entered does not contain an upper case letter.</returns>
        public override IdentityError PasswordRequiresUpper()
        {
            return CreateNamedError(nameof(PasswordRequiresUpper));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a password of the specified <paramref name="length" /> does not meet the minimum length requirements.
        /// </summary>
        /// <param name="length">The length that is not long enough.</param>
        /// <returns>An <see cref="IdentityError" /> indicating a password of the specified <paramref name="length" /> does not meet the minimum length requirements.</returns>
        public override IdentityError PasswordTooShort(int length)
        {
            return CreateNamedError(nameof(PasswordTooShort));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a recovery code was not redeemed.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating a recovery code was not redeemed.</returns>
        public override IdentityError RecoveryCodeRedemptionFailed()
        {
            return CreateNamedError(nameof(RecoveryCodeRedemptionFailed));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a user already has a password.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating a user already has a password.</returns>
        public override IdentityError UserAlreadyHasPassword()
        {
            return CreateNamedError(nameof(UserAlreadyHasPassword));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a user is already in the specified <paramref name="role" />.
        /// </summary>
        /// <param name="role">The duplicate role.</param>
        /// <returns>An <see cref="IdentityError" /> indicating a user is already in the specified <paramref name="role" />.</returns>
        public override IdentityError UserAlreadyInRole(string role)
        {
            return CreateNamedError(nameof(UserAlreadyInRole));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating user lockout is not enabled.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating user lockout is not enabled.</returns>
        public override IdentityError UserLockoutNotEnabled()
        {
            return CreateNamedError(nameof(UserLockoutNotEnabled));
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a user is not in the specified <paramref name="role" />.
        /// </summary>
        /// <param name="role">The duplicate role.</param>
        /// <returns>An <see cref="IdentityError" /> indicating a user is not in the specified <paramref name="role" />.</returns>
        public override IdentityError UserNotInRole(string role)
        {
            return CreateNamedError(nameof(UserNotInRole));
        }

        /// <summary>
        /// Generates <see cref="IdentityError"/> with <see cref="IdentityError.Code"/> named as <paramref name="name"/>
        /// And error description which uses <paramref name="name"/> as lookup key for <see cref="IStringLocalizer{T}"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns>An <see cref="IdentityError"/> that is used in another method return with specific indication for what this error was created</returns>
        private IdentityError CreateNamedError(string name)
        {
            return new IdentityError
            {
                Code = name,
                Description = _stringLocalizer.GetString(name)
            };
        }
    }
}
