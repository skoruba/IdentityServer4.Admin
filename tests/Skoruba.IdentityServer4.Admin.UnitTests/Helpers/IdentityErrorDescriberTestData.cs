using Skoruba.IdentityServer4.Admin.Helpers.Identity;
using System.Collections;
using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Helpers
{
    public class IdentityErrorDescriberTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            #region Parameterless methods

            yield return new object[]
            {
                nameof(IdentityErrorMessages.ConcurrencyFailure),
                nameof(IdentityErrorMessages.ConcurrencyFailure),
                "ConcurrencyFailureTranslated"
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.DefaultError),
                nameof(IdentityErrorMessages.DefaultError),
                "DefaultErrorTranslated"
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.PasswordRequiresNonAlphanumeric),
                nameof(IdentityErrorMessages.PasswordRequiresNonAlphanumeric),
                "PasswordRequiresNonAlphanumericTranslated"
                };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.LoginAlreadyAssociated),
                nameof(IdentityErrorMessages.LoginAlreadyAssociated),
                "LoginAlreadyAssociatedTranslated"
            };


            yield return new object[]
            {
                nameof(IdentityErrorMessages.InvalidToken),
                nameof(IdentityErrorMessages.InvalidToken),
                "InvalidTokenTranslated",
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.PasswordMismatch),
                nameof(IdentityErrorMessages.PasswordMismatch),
                "PasswordMismatchTranslated"
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.PasswordRequiresDigit),
                nameof(IdentityErrorMessages.PasswordRequiresDigit),
                "PasswordRequiresDigitTranslated"
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.PasswordRequiresLower),
                nameof(IdentityErrorMessages.PasswordRequiresLower),
                "PasswordRequiresLowerTranslated"
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.RecoveryCodeRedemptionFailed),
                nameof(IdentityErrorMessages.RecoveryCodeRedemptionFailed),
                "RecoveryCodeRedemptionFailedTranslated"
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.UserAlreadyHasPassword),
                nameof(IdentityErrorMessages.UserAlreadyHasPassword),
                "UserAlreadyHasPasswordTranslated"
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.UserLockoutNotEnabled),
                nameof(IdentityErrorMessages.UserLockoutNotEnabled),
                "UserLockoutNotEnabledTranslated"
            };


            #endregion

            #region Methods with parameters

            yield return new object[]
            {
                nameof(IdentityErrorMessages.InvalidEmail),
                nameof(IdentityErrorMessages.InvalidEmail),
                "InvalidEmailTranslated",
                "TestUsername"
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.DuplicateUserName),
                nameof(IdentityErrorMessages.DuplicateUserName),
                "DuplicateUserNameTranslated",
                "TestDuplicatedUsername"
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.DuplicateRoleName),
                nameof(IdentityErrorMessages.DuplicateRoleName),
                "DuplicateRoleNameTranslated",
                "TestRoleName"
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.InvalidRoleName),
                nameof(IdentityErrorMessages.InvalidRoleName),
                "InvalidRoleNameTranslated",
                "InvalidRoleNameTest"
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.PasswordTooShort),
                nameof(IdentityErrorMessages.PasswordTooShort),
                "PasswordTooShortTranslated",
                4
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.PasswordRequiresUpper),
                nameof(IdentityErrorMessages.PasswordRequiresUpper),
                "PasswordRequiresUpperTranslated",
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.UserAlreadyInRole),
                nameof(IdentityErrorMessages.UserAlreadyInRole),
                "UserAlreadyInRoleTranslated",
                "TestRole"
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.UserNotInRole),
                nameof(IdentityErrorMessages.UserNotInRole),
                "UserNotInRoleTranslated",
                "TestRole"
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.InvalidUserName),
                nameof(IdentityErrorMessages.InvalidUserName),
                "InvalidUsernameTranslated",
                "TestUsername"
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.PasswordRequiresUniqueChars),
                nameof(IdentityErrorMessages.PasswordRequiresUniqueChars),
                "PasswordRequiresUniqueCharsTranslated",
                5
            };

            yield return new object[]
            {
                nameof(IdentityErrorMessages.DuplicateEmail),
                nameof(IdentityErrorMessages.DuplicateEmail),
                "DuplicateEmailTranslated",
                "testduplicateemail@email.com"
            };

            #endregion
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
