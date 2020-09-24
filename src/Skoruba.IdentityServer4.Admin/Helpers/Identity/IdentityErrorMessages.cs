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
            if (TryCreateLocalizedError(nameof(ConcurrencyFailure), out var error))
            {
                return error;
            }

            return base.ConcurrencyFailure();
        }

        /// <summary>
        /// Returns the default <see cref="IdentityError" />.
        /// </summary>
        /// <returns>The default <see cref="IdentityError" />.</returns>
        public override IdentityError DefaultError()
        {
            if (TryCreateLocalizedError(nameof(DefaultError), out var error))
            {
                return error;
            }

            return base.DefaultError();
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating the specified <paramref name="email" /> is already associated with an account.
        /// </summary>
        /// <param name="email">The email that is already associated with an account.</param>
        /// <returns>An <see cref="IdentityError" /> indicating the specified <paramref name="email" /> is already associated with an account.</returns>
        public override IdentityError DuplicateEmail(string email)
        {
            if (TryCreateLocalizedError(nameof(DuplicateEmail), out var error, email))
            {
                return error;
            }

            return base.DuplicateEmail(email);
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating the specified <paramref name="role" /> name already exists.
        /// </summary>
        /// <param name="role">The duplicate role.</param>
        /// <returns>An <see cref="IdentityError" /> indicating the specific role <paramref name="role" /> name already exists.</returns>
        public override IdentityError DuplicateRoleName(string role)
        {
            if (TryCreateLocalizedError(nameof(DuplicateRoleName), out var error, role))
            {
                return error;
            }

            return base.DuplicateRoleName(role);
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating the specified <paramref name="userName" /> already exists.
        /// </summary>
        /// <param name="userName">The user name that already exists.</param>
        /// <returns>An <see cref="IdentityError" /> indicating the specified <paramref name="userName" /> already exists.</returns>
        public override IdentityError DuplicateUserName(string userName)
        {
            if (TryCreateLocalizedError(nameof(DuplicateUserName), out var error, userName))
            {
                return error;
            }

            return base.DuplicateUserName(userName);
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating the specified <paramref name="email" /> is invalid.
        /// </summary>
        /// <param name="email">The email that is invalid.</param>
        /// <returns>An <see cref="IdentityError" /> indicating the specified <paramref name="email" /> is invalid.</returns>
        public override IdentityError InvalidEmail(string email)
        {
            if (TryCreateLocalizedError(nameof(InvalidEmail), out var error, email))
            {
                return error;
            }

            return base.InvalidEmail(email);
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating the specified <paramref name="role" /> name is invalid.
        /// </summary>
        /// <param name="role">The invalid role.</param>
        /// <returns>An <see cref="IdentityError" /> indicating the specific role <paramref name="role" /> name is invalid.</returns>
        public override IdentityError InvalidRoleName(string role)
        {
            if (TryCreateLocalizedError(nameof(InvalidRoleName), out var error, role))
            {
                return error;
            }

            return base.InvalidRoleName(role);
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating an invalid token.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating an invalid token.</returns>
        public override IdentityError InvalidToken()
        {
            if (TryCreateLocalizedError(nameof(InvalidToken), out var error))
            {
                return error;
            }

            return base.InvalidToken();
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating the specified user <paramref name="userName" /> is invalid.
        /// </summary>
        /// <param name="userName">The user name that is invalid.</param>
        /// <returns>An <see cref="IdentityError" /> indicating the specified user <paramref name="userName" /> is invalid.</returns>
        public override IdentityError InvalidUserName(string userName)
        {
            if (TryCreateLocalizedError(nameof(InvalidUserName), out var error, userName))
            {
                return error;
            }

            return base.InvalidUserName(userName);
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating an external login is already associated with an account.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating an external login is already associated with an account.</returns>
        public override IdentityError LoginAlreadyAssociated()
        {
            if (TryCreateLocalizedError(nameof(LoginAlreadyAssociated), out var error))
            {
                return error;
            }

            return base.LoginAlreadyAssociated();
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a password mismatch.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating a password mismatch.</returns>
        public override IdentityError PasswordMismatch()
        {
            if (TryCreateLocalizedError(nameof(PasswordMismatch), out var error))
            {
                return error;
            }

            return base.PasswordMismatch();
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a password entered does not contain a numeric character, which is required by the password policy.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating a password entered does not contain a numeric character.</returns>
        public override IdentityError PasswordRequiresDigit()
        {
            if (TryCreateLocalizedError(nameof(PasswordRequiresDigit), out var error))
            {
                return error;
            }

            return base.PasswordRequiresDigit();
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a password entered does not contain a lower case letter, which is required by the password policy.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating a password entered does not contain a lower case letter.</returns>
        public override IdentityError PasswordRequiresLower()
        {
            if (TryCreateLocalizedError(nameof(PasswordRequiresLower), out var error))
            {
                return error;
            }

            return base.PasswordRequiresLower();
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a password entered does not contain a non-alphanumeric character, which is required by the password policy.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating a password entered does not contain a non-alphanumeric character.</returns>
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            if (TryCreateLocalizedError(nameof(PasswordRequiresNonAlphanumeric), out var error))
            {
                return error;
            }

            return base.PasswordRequiresNonAlphanumeric();
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a password does not meet the minimum number <paramref name="uniqueChars" /> of unique chars.
        /// </summary>
        /// <param name="uniqueChars">The number of different chars that must be used.</param>
        /// <returns>An <see cref="IdentityError" /> indicating a password does not meet the minimum number <paramref name="uniqueChars" /> of unique chars.</returns>
        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            if (TryCreateLocalizedError(nameof(PasswordRequiresUniqueChars), out var error, uniqueChars))
            {
                return error;
            }

            return base.PasswordRequiresUniqueChars(uniqueChars);
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a password entered does not contain an upper case letter, which is required by the password policy.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating a password entered does not contain an upper case letter.</returns>
        public override IdentityError PasswordRequiresUpper()
        {
            if (TryCreateLocalizedError(nameof(PasswordRequiresUpper), out var error))
            {
                return error;
            }

            return base.PasswordRequiresUpper();
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a password of the specified <paramref name="length" /> does not meet the minimum length requirements.
        /// </summary>
        /// <param name="length">The length that is not long enough.</param>
        /// <returns>An <see cref="IdentityError" /> indicating a password of the specified <paramref name="length" /> does not meet the minimum length requirements.</returns>
        public override IdentityError PasswordTooShort(int length)
        {
            if (TryCreateLocalizedError(nameof(PasswordTooShort), out var error, length))
            {
                return error;
            }

            return base.PasswordTooShort(length);
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a recovery code was not redeemed.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating a recovery code was not redeemed.</returns>
        public override IdentityError RecoveryCodeRedemptionFailed()
        {
            if (TryCreateLocalizedError(nameof(RecoveryCodeRedemptionFailed), out var error))
            {
                return error;
            }

            return base.RecoveryCodeRedemptionFailed();
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a user already has a password.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating a user already has a password.</returns>
        public override IdentityError UserAlreadyHasPassword()
        {
            if (TryCreateLocalizedError(nameof(UserAlreadyHasPassword), out var error))
            {
                return error;
            }

            return base.UserAlreadyHasPassword();
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a user is already in the specified <paramref name="role" />.
        /// </summary>
        /// <param name="role">The duplicate role.</param>
        /// <returns>An <see cref="IdentityError" /> indicating a user is already in the specified <paramref name="role" />.</returns>
        public override IdentityError UserAlreadyInRole(string role)
        {
            if (TryCreateLocalizedError(nameof(UserAlreadyInRole), out var error, role))
            {
                return error;
            }

            return base.UserAlreadyInRole(role);
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating user lockout is not enabled.
        /// </summary>
        /// <returns>An <see cref="IdentityError" /> indicating user lockout is not enabled.</returns>
        public override IdentityError UserLockoutNotEnabled()
        {
            if (TryCreateLocalizedError(nameof(UserLockoutNotEnabled), out var error))
            {
                return error;
            }

            return base.UserLockoutNotEnabled();
        }

        /// <summary>
        /// Returns an <see cref="IdentityError" /> indicating a user is not in the specified <paramref name="role" />.
        /// </summary>
        /// <param name="role">The duplicate role.</param>
        /// <returns>An <see cref="IdentityError" /> indicating a user is not in the specified <paramref name="role" />.</returns>
        public override IdentityError UserNotInRole(string role)
        {
            if (TryCreateLocalizedError(nameof(UserNotInRole), out var error, role))
            {
                return error;
            }

            return base.UserNotInRole(role);
        }

        /// <summary>
        /// Tries to generate <see cref="IdentityError"/> with <see cref="IdentityError.Code"/> named as <paramref name="name"/>
        /// And error description which uses <paramref name="name"/> as lookup key for <see cref="IStringLocalizer{T}"/>
        /// </summary>
        /// <param name="name">Key used as lookup key for <see cref="IStringLocalizer{T}"/></param>
        /// <returns><see cref="bool"/> representing that localized error was created successfully</returns>
        private bool TryCreateLocalizedError(in string name, out IdentityError error, params object[] formatArgs)
        {
            LocalizedString description = _stringLocalizer.GetString(name, formatArgs);
            if (description.ResourceNotFound)
            {
                error = new IdentityError();
                return false;
            }

            error = new IdentityError
            {
                Code = name,
                Description = description
            };
            return true;
        }
    }
}
