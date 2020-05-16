using Skoruba.DbMigrator.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Skoruba.DbMigrator.Abstractions.Dependency
{
    public class DependencyItem
    {
        private readonly List<DependencyItem> _dependencyItems;
        public object Instance { get; }
        public Type Type { get; }
        public IReadOnlyList<DependencyItem> Dependencies => _dependencyItems.ToImmutableList();

        public DependencyItem(Type type, object instance)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));

            Instance = instance ?? throw new ArgumentNullException(nameof(instance));

            _dependencyItems = new List<DependencyItem>();
        }


        public override string ToString()
        {
            return Type.ToString();
        }

        public void AddDependency(DependencyItem item)
        {
            _dependencyItems.AddIfNotContains(item);
        }
    }
}
