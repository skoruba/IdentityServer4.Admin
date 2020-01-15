// Copyright 2019 Andrew White
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.EntityFrameworkCore.Metadata;

namespace Skoruba.MultiTenant.EntityFrameworkCore
{
    public static class EntityTypeExtensions
    {
        /// <summary>
        /// Whether or not the <see cref="IEntityType"/> is configured as MultiTenant.
        /// </summary>
        /// <param name="entityType">The entity type to test for MultiTenant configuration.</param>
        /// <returns><see cref="true"/> if the entity type has MultiTenant configuration, <see cref="false"/> if not.</returns>
        public static bool IsMultiTenant(this IEntityType entityType)
        {
            return (bool?)entityType.FindAnnotation(Constants.MultiTenantAnnotationName)?.Value ?? false;
        }
    }
}
