using System;

namespace Skoruba.DbMigrator.Abstractions.Dependency
{
    public interface IDependedTypesProvider
    {
        Type[] GetDependedTypes();
    }
}
