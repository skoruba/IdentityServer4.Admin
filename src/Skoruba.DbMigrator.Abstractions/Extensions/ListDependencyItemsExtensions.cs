using Skoruba.DbMigrator.Abstractions.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Skoruba.DbMigrator.Abstractions.Extensions
{
    public static class ListDependencyItemsExtensions
    {
        public static List<DependencyItem> SetDependencies(this List<DependencyItem> items)
        {
            foreach (var item in items)
            {
                SetDependencies(items, item);
            }

            return items;
        }

        private static void SetDependencies(IEnumerable<DependencyItem> items, DependencyItem item)
        {
            foreach (var dependedModuleType in FindDependedModuleTypes(item.Type))
            {
                var dependedModule = items.FirstOrDefault(m => m.Type == dependedModuleType);
                if (dependedModule == null)
                {
                    throw new Exception("Could not find a depended item " + dependedModuleType.AssemblyQualifiedName + " for " + item.Type.AssemblyQualifiedName);
                }

                item.AddDependency(dependedModule);
            }
        }

        private static List<Type> FindDependedModuleTypes(Type moduleType)
        {
            var dependencies = new List<Type>();

            var dependencyDescriptors = moduleType
                .GetCustomAttributes()
                .OfType<IDependedTypesProvider>();

            foreach (var descriptor in dependencyDescriptors)
            {
                foreach (var dependedModuleType in descriptor.GetDependedTypes())
                {
                    dependencies.AddIfNotContains(dependedModuleType);
                }
            }
            return dependencies;
        }

    }
}
