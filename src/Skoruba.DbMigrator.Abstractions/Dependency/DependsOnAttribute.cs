using Skoruba.DbMigrator.Abstractions.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Skoruba.DbMigrator.Abstractions.Dependency
{
    /// <summary>
    /// Register types that this object depends on
    /// </summary>
    /// <remarks>
    /// Concept was influenced by AbpFramework 
    /// https://github.com/abpframework/abp/blob/dev/framework/src/Volo.Abp.Core/Volo/Abp/Modularity/DependsOnAttribute.cs
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependsOnAttribute : Attribute, IDependedTypesProvider
    {
        public DependsOnAttribute(params Type[] types)
        {
            DependedTypes = types ?? new Type[0];
        }
        public DependsOnAttribute(params string[] types)
        {
            DependedTypes = types
                .Select(t =>Type.GetType(t)?? AppDomain.CurrentDomain.GetTypeByFullName(t))
                .Where(t => t != null)
                .ToArray();
        }

        [NotNull]
        public Type[] DependedTypes { get; }

        public virtual Type[] GetDependedTypes()
        {
            return DependedTypes;
        }
    }
}
