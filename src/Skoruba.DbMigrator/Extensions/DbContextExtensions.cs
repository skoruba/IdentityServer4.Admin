using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;

namespace Skoruba.DbMigrator.Extensions
{
    public static class DbContextExtensions
    {
        public static void EnableIdentityInsert(this DbContext context, IEntityType entityType, string pkName = "Id") => SetIdentityInsert(context, entityType, enable: true, pkName: pkName);

        public static void DisableIdentityInsert(this DbContext context, IEntityType entityType, string pkName = "Id") => SetIdentityInsert(context, entityType, enable: false, pkName: pkName);

        private static void SetIdentityInsert(DbContext context, IEntityType entityType, bool enable, string pkName)
        {
            var isIdentity = entityType.FindProperty(pkName).ValueGenerated;

            if (isIdentity == ValueGenerated.Never) return;

            var value = enable ? "ON" : "OFF";
            var tableName = entityType.GetTableName();
#pragma warning disable EF1000 // Possible SQL injection vulnerability.
            //NOTE: Using the string interpolation directly in the ExecuteSqlCommand threw an error.
            var commandText = $"SET IDENTITY_INSERT dbo.{tableName} {value}";
            _ = context.Database.ExecuteSqlRaw(commandText);
#pragma warning restore EF1000 // Possible SQL injection vulnerability.
        }

    }
}
